using System;
using TMPro;
using UnityEngine;

namespace MCG.UnityCheatSheet
{
    public class TimerBehaviourScript : MonoBehaviour
    {
        // --------- Timer ---------
        /*
        
        1 -: your code has to have
            
                using System;
                using UnityEngine.UI;
                using TMPro;
    
              on the top of the file in order for the timer to work.
    
        2 -: you need to download and import TextMeshPro from the Package Manager (on the editor go
             to Window -> Package Manager -> click on "Packages:" and select "Unity Registry" ->
             search TextMeshPro -> Download (if shown) -> Import (if shown))
    
        3 -: your timer text can't be a normal UI->Text, it has to be UI->TextMeshPro
    
    
        use StartTimer(), PauseTimer() and PlayTimer() to use the file.
        drag the TextMeshPro Object to timeText on your TimeScript Component
    
        */
        [SerializeField]
        private TMP_Text timeText;
        private float TotalTime = 0.0f;
        private float TimeStarted = 0.0f;

        // public bool game_started = false;
        public bool game_started = false;

        private void Start()
        {
            StartTimer();
        }

        private void Update()
        {
            AppendTime();
        }

        public float GetTotalTimeInMinutes()
        {
            //       mili     second  minute
            return TotalTime / 60f;
        }

        // [SerializeField] private TMP_Text timeText; // in here goes the text UI element for the timer

        public void SetTimeStarted()
        {
            TimeStarted = Time.realtimeSinceStartup;
        }

        // should be called whenever the timer should play (like after pause menu)
        public void PlayTimer()
        {
            game_started = true;
            SetTimeStarted();
        }

        // should be called whenever the timer should stop (like at pause menu)
        public void PauseTimer()
        {
            game_started = false;
        }

        // should be called whenever the timer should restart (like at the first frame)
        public void StartTimer()
        {
            SetTimeStarted();
            TotalTime = 0;
            game_started = true;
        }

        // can be called to get the timer without restarting (like after pause)
        public void StartTimerWithoutResetingTime()
        {
            SetTimeStarted();
            game_started = true;
        }

        // should be called every frame. sets timeText's text to be TotalTime of timer
        public void AppendTime()
        {
            if (!game_started)
                return;
            TotalTime += Time.realtimeSinceStartup - TimeStarted;
            SetTimeStarted();
            timeText.text = FixFloatDecimal(TotalTime);
        }

        // Gets time in milliseconds and transform it to mins:secs:decs
        private string FixFloatDecimal(float dec)
        {
            float decs = (int)((dec - (int)dec) * 100);
            float secs = TimeSpan.FromSeconds(dec).Seconds;
            float mins = TimeSpan.FromSeconds(dec).Minutes;
            // float hours = TimeSpan.FromSeconds(dec).Hours;


            string ans = "";
            // ans += SetStringForTimer(hours) + ":";
            ans += SetStringForTimer(mins) + ":";
            ans += SetStringForTimer(secs) + ".";
            ans += "<size=80%>" + SetStringForTimer(decs); // makes the decimal to be smaller
            return ans;
        }

        // padding zeros to make 0 to 00 and 1...9 to 01...09
        string SetStringForTimer(float f)
        {
            return f >= 10
                ? f.ToString()
                : f > 0
                    ? "0" + f.ToString()
                    : "00";
        }

        // ---------------------------
    }
}
