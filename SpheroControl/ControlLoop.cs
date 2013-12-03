//###################################################################################################
/*
    Copyright (c) since 2013 - Paul Freund 
    
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:
    
    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
//###################################################################################################

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;

using XInputWrapper;

namespace SpheroControl
{
    public class ControlLoop
    {
        public ControlLoop() { }

        private SpheroWrapper _sphero = new SpheroWrapper();
        private Gamepad _gamepad = new Gamepad();

        private TimeSpan _frequencyReconnect = TimeSpan.FromMilliseconds(10000); // 10s
        private TimeSpan _frequencyPoll = TimeSpan.FromMilliseconds(1000 / 60); // 60Hz

        private ThreadPoolTimer _loopPoll;
        private DispatcherTimer _loopReconnect;
        private bool _runningPoll = false;
        private bool _runningReconnect = false;

        private GamepadData _currentData;

        #region API

        public void Start()
        {
            Stop();
            if (_loopReconnect != null || _loopPoll != null) return;

            if (!_sphero.IsConnected)
                _sphero.Connect();

            _loopPoll = ThreadPoolTimer.CreatePeriodicTimer(FunctionPoll, _frequencyPoll);
            _loopReconnect =  new DispatcherTimer();
            _loopReconnect.Interval = _frequencyReconnect;
            _loopReconnect.Tick += FunctionReconnect;
            _loopReconnect.Start();
        }

        public void Stop()
        {
            if (_loopPoll != null)
            {
                _loopPoll.Cancel();
                _loopPoll = null;
            }

            if (_loopReconnect != null)
            {
                _loopReconnect.Stop();
                _loopReconnect = null;
            }

            _sphero.Disconnect();
        }

        public bool IsRunning { get { return (_loopReconnect != null && _loopPoll != null); } }

        #endregion

        private void FunctionReconnect(object sender, object e)
        {
            if (_runningReconnect) return;
            _runningReconnect = true;

            if (!_sphero.IsConnected)
            {
                _sphero.Connect();
            }
            else
            {
                _sphero._sphero.SetBackLED(1.0f);
                _sphero._sphero.SetRGBLED(0, 0, 0);
            }

            _runningReconnect = false;
        }

        private void FunctionPoll(ThreadPoolTimer timer)
        {
            if (_runningPoll) return;
            _runningPoll = true;

            if (_sphero.IsConnected)
            {
                _currentData = _gamepad.Poll();

                if (_currentData.ControllerConnected)
                {
                    if (_currentData.ButtonA)
                    {
                        _sphero._sphero.SetHeading(0);
                    }
                    else if (_currentData.ButtonB)
                    {
                        _sphero._sphero.SetHeading(180);
                    }
                    else if (_currentData.ButtonX)
                    {
                        _sphero._sphero.SetHeading(0);
                    }
                    else if (_currentData.ButtonY)
                    {
                        _sphero._sphero.SetHeading(0);
                    }
                    else
                    {
                        if (_currentData.RightIntensity > 0.5f)
                        {
                            float inverseAngle = _currentData.RightAngle + 180.0f;
                            inverseAngle = (inverseAngle <= 360.0f ? inverseAngle : (inverseAngle - 360.0f));
                            _sphero._sphero.Roll((int)(inverseAngle + 0.5f), 0.0f);
                        }
                        else
                        {
                            _currentData.RightTrigger = _currentData.RightTrigger > 0.01f ? _currentData.RightTrigger : 0.0f;
                            _sphero._sphero.Roll((int)(_currentData.LeftAngle + 0.5f), _currentData.RightTrigger);
                        }
                    }
                    //if (_currentData.ButtonB)
                    //    _sphero._sphero.SetBackLED(_currentData.LeftTrigger);

                    //if (_currentData.ButtonY)
                    //{
                    //    _sphero._sphero.SetRGBLED(
                    //        (int)((_currentData.LeftTrigger * 255) + 0.5),
                    //        (int)((_currentData.RightTrigger * 255) + 0.5),
                    //        (int)((_currentData.RightIntensity * 255) + 0.5)
                    //    );
                    //}
                }
            }

            _runningPoll = false;
        }
    }
}
