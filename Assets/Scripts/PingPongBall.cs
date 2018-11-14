using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MsgReceiver;

public class PingPongBall : MonoBehaviour {
    private float speed = 2;
    public bool debug_input = true;

    private Receiver receiver = new Receiver();
    private TrackInfo trackInfo;
    
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject);

        //start the receiver if not in debug mode
        if(debug_input != true)
        {
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
            transform.Translate(TrackInfoTransformToGame(trackInfo));
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
            trackInfo = e.track;
        }
    }

    private bool ValidateTrackInfo(TrackInfo info)
    {
        //if all ball info equals to zero then its not valid
        if((info.ball_X_px == 0) && (info.ball_Y_px==0) && (info.ball_Z_mm==0))
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
        //need calibration
        //todo
        pos.x = info.ball_X_px;
        pos.y = info.ball_Y_px;
        pos.z = 0;//fixed for now

        return pos;
    }
}
