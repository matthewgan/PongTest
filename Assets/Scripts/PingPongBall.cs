using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using MsgReceiver;

public class PingPongBall : MonoBehaviour {
    private float speed = 2;
    public bool debug_input = false;

    private Receiver receiver = new Receiver();
    private TrackInfo last_trackInfo;

    private Vector2 real_table_min = new Vector2();
    private Vector2 real_table_max = new Vector2();

    public Vector2 virtual_table_min = new Vector2(-4.0f, -2.0f);
    public Vector2 virtual_table_max = new Vector2(4.0f, 2.0f);


    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(gameObject);       

        LoadConfig.LoadSettings(System.Environment.CurrentDirectory);

        CalculateTablePixelRange();

        //start the receiver if not in debug mode
        if(debug_input != true)
        {
            last_trackInfo = new TrackInfo();

            receiver = new Receiver();
            receiver.Open();
            receiver.MsgRecieved += UpdateTrackInfo;
        }
	}

    // Update is called once per frame
    void Update () {
        //when kinect is not connected, using debug keyboard input
        if(debug_input)
        {
            Vector3 pos = KeyboardInputBall();
            transform.Translate(pos);
        }
        else
        {
            //real position update from kinect tracker
            var pos = TrackInfoTransformToGame(last_trackInfo);
            //Debug.Log(pos);
            transform.position = pos;
        }
    }

    private Vector3 KeyboardInputBall()
    {
        // 定义3个值控制移动
        float xm = 0, ym = 0;
        //按键盘W向上移动
        if (Input.GetKey(KeyCode.W))
        {
            ym += speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))//按键盘S向下移动
        {
            ym -= speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))//按键盘A向左移动
        {
            xm -= speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))//按键盘D向右移动
        {
            xm += speed * Time.deltaTime;
        }
        return new Vector3(xm, ym, 0);
    }

    private void UpdateTrackInfo(object sender, Receiver.MsgEventArgs e)
    {
        if (ValidateTrackInfo(e.track))
        {
            last_trackInfo = e.track;
        }
    }

    private bool ValidateTrackInfo(TrackInfo info)
    {
        //if all ball info equals to zero then its not valid
        if((info.ball_X_px == 0) && (info.ball_Y_px==0) && (info.ball_Z_m==0))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Vector3 TrackInfoTransformToGame(TrackInfo info)
    {
        Vector3 pos = new Vector3();
        //need calibration from setting file
        //get table size in the setting file
        pos.x = (info.ball_X_px - real_table_min.x) * (virtual_table_max.x - virtual_table_min.x) / (real_table_max.x - real_table_min.x) + virtual_table_min.x;
        pos.y = virtual_table_max.y - (info.ball_Y_px - real_table_min.y) * (virtual_table_max.y - virtual_table_min.y) / (real_table_max.y - real_table_min.y);
        pos.z = 0;

        return pos;
    }

    private void CalculateTablePixelRange()
    {
        real_table_min.x = Mathf.Min(LoadConfig.gameSettings.pingpong_table_tl[0], LoadConfig.gameSettings.pingpong_table_bl[0]);
        real_table_min.y = Mathf.Min(LoadConfig.gameSettings.pingpong_table_tl[1], LoadConfig.gameSettings.pingpong_table_tr[1]);
        real_table_max.x = Mathf.Max(LoadConfig.gameSettings.pingpong_table_tr[0], LoadConfig.gameSettings.pingpong_table_br[0]);
        real_table_max.y = Mathf.Max(LoadConfig.gameSettings.pingpong_table_bl[1], LoadConfig.gameSettings.pingpong_table_br[1]);
    }
}
