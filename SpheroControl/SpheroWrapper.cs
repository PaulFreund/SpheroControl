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
using System.Diagnostics;

using RobotKit;

namespace SpheroControl
{
    public class SpheroWrapper
    {
        private RobotProvider _provider = RobotProvider.GetSharedProvider();

        // Temporary public for testing
        public Sphero _sphero;

        public SpheroWrapper() 
        {
            _provider.NoRobotsEvent += _provider_NoRobotsEvent;
            _provider.DiscoveredRobotEvent += _provider_DiscoveredRobotEvent;
            _provider.ConnectedRobotEvent += _provider_ConnectedRobotEvent;
        }

        ~SpheroWrapper()
        {
            _provider.NoRobotsEvent -= _provider_NoRobotsEvent;
            _provider.DiscoveredRobotEvent -= _provider_DiscoveredRobotEvent;
            _provider.ConnectedRobotEvent -= _provider_ConnectedRobotEvent;
        }

        #region API

        public bool IsConnected { get { return (_sphero != null); } }

        public void Connect()
        {
            if (IsConnected) return;
            Disconnect();

            _provider.FindRobots();
        }

        public void Disconnect(bool sleep = false)
        {
            if (IsConnected) 
            {
                if (_sphero.SensorControl != null)
                {
                    _sphero.SensorControl.StopAll();
                    _sphero.SensorControl.VelocityUpdatedEvent -= SensorControl_VelocityUpdatedEvent;
                    _sphero.SensorControl.QuaternionUpdatedEvent -= SensorControl_QuaternionUpdatedEvent;
                    _sphero.SensorControl.LocationUpdatedEvent -= SensorControl_LocationUpdatedEvent;
                    _sphero.SensorControl.AttitudeUpdatedEvent -= SensorControl_AttitudeUpdatedEvent;
                    _sphero.SensorControl.AccelerometerUpdatedEvent -= SensorControl_AccelerometerUpdatedEvent;
                    _sphero.SensorControl.GyrometerUpdatedEvent -= SensorControl_GyrometerUpdatedEvent;
                }

                if(_sphero.CollisionControl != null)
                {
                    _sphero.CollisionControl.StopDetection();
                    _sphero.CollisionControl.CollisionDetectedEvent -= CollisionControl_CollisionDetectedEvent;
                }

                if (sleep)
                    _sphero.Sleep();
                else
                    _sphero.Disconnect();
            }

            _provider.DisconnectAll();
            _sphero = null;
        }

        #endregion

        #region Events

        private void _provider_NoRobotsEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("[NoRobots]");
        }

        private void _provider_DiscoveredRobotEvent(object sender, Robot e) 
        {
            if (_sphero != null || e == null) return;

            Debug.WriteLine("[DiscoveredRobot] " + e.Name + "(" + e.BluetoothName + ")");

            _provider.ConnectRobot(e);
        }

        private void _provider_ConnectedRobotEvent(object sender, Robot e) 
        {
            Debug.WriteLine("[ConnectedRobot] " + e.Name + "(" + e.BluetoothName + ")");

            _sphero = e as Sphero;
            if (_sphero == null)
                return;

            _sphero.SensorControl.Hz = 1;
            _sphero.SensorControl.VelocityUpdatedEvent += SensorControl_VelocityUpdatedEvent;
            _sphero.SensorControl.QuaternionUpdatedEvent += SensorControl_QuaternionUpdatedEvent;
            _sphero.SensorControl.LocationUpdatedEvent += SensorControl_LocationUpdatedEvent;
            _sphero.SensorControl.AttitudeUpdatedEvent += SensorControl_AttitudeUpdatedEvent;
            _sphero.SensorControl.AccelerometerUpdatedEvent += SensorControl_AccelerometerUpdatedEvent;
            _sphero.SensorControl.GyrometerUpdatedEvent += SensorControl_GyrometerUpdatedEvent;

            _sphero.CollisionControl.StartDetectionForWallCollisions();
            _sphero.CollisionControl.CollisionDetectedEvent += CollisionControl_CollisionDetectedEvent;
        }

        #endregion

        #region Sphero_Events

        private void SensorControl_VelocityUpdatedEvent(object sender, VelocityReading e)
        {
            Debug.WriteLine("[VELOCITY] X: " + e.X + " Y: " + e.Y);
        }

        private void SensorControl_QuaternionUpdatedEvent(object sender, QuaternionReading e)
        {
            Debug.WriteLine("[QUATERNION] X: " + e.X + " Y: " + e.Y + " Z: " + e.Z + " W: " + e.W);
        }

        private void SensorControl_LocationUpdatedEvent(object sender, LocationReading e)
        {
            Debug.WriteLine("[LOCATION] X: " + e.X + " Y: " + e.Y);
        }

        private void SensorControl_AttitudeUpdatedEvent(object sender, AttitudeReading e)
        {
            Debug.WriteLine("[ATTITUDE] Roll: " + e.Roll + " Yaw: " + e.Yaw + " Pitch: " + e.Pitch);
        }

        private void SensorControl_AccelerometerUpdatedEvent(object sender, AccelerometerReading e)
        {
            Debug.WriteLine("[ACCELEROMETER] X: " + e.X + " Y: " + e.Y + " Z: " + e.Z);
        }

        private void SensorControl_GyrometerUpdatedEvent(object sender, GyrometerReading e)
        {
            Debug.WriteLine("[GYROMETER] X: " + e.X + " Y: " + e.Y + " Z: " + e.Z);
        }

        private void CollisionControl_CollisionDetectedEvent(object sender, CollisionData e)
        {
            Debug.WriteLine("[COLLISION]");
        }

        #endregion
    }
}
