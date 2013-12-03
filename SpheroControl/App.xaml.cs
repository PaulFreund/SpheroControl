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

/////////////////////////////////////////////////////////////////////////////
// Todo
/////////////////////////////////////////////////////////////////////////////

////// General

//// UI
// Display if Sphero is connected
// Display if Gamepad is connected
// Setting of input type
// Manual color picker
// Touch movement
// (accelerometer movement?)
// Change of settings (including modes and behaviour)
// If possible gamepad mapper

//// Sphero
// Sphero events don't work at the moment
// Pass more information to loop

//// Input
// Multiple input types (touch/keyboard/gamepad)
// Gamepad Vibration
// Better support for other gamepads

////// Modes (Gamepad)

//// Direction Mode
// Left stick defines direction
// Right Trigger defines speed
// A is nitro button (no full speed by trigger)
// Y switches between default color and color coding of speed
// X enables calibration while pressed (enable calibration light, set Speed to 0 and use left stick angle value when released )
// B toggles calibration light 
// Right Stick defines Color (if enabled)

//// Move Mode
// Left stick defines direction and speed 
// A is nitro button (no full speed by trigger)
// Y switches between default color and color coding of speed
// X enables calibration while pressed (enable calibration light, set Speed to 0 and use left stick angle value when released )
// B toggles calibration light 
// Right Stick defines Color (if enabled)

//// Car Mode
// Right Trigger increase speed (accelerate)
// Left Trigger decrease speed  
// Left stick steering left and right (only X-axis of stick)
// Calibration light always on
// Y switches between default color and color coding of speed
// Color animation on colision
// A is nitro button (no full speed by acceleration)

//// Tank mode
// Left trigger defines half of speed and adds an offset to move to the left ( up to 90deg )
// Right trigger defines half of the speed and adds an offset to move to the right ( up to 90deg )
// Calibration light always on
// Y switches between default color and color coding of speed
// Color animation on colision

using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SpheroControl
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public readonly ControlLoop _controlLoop = new ControlLoop();

        public CoreWindow CoreWindow
        {
            get
            {
                if (Window.Current != null && Window.Current.CoreWindow != null)
                    return Window.Current.CoreWindow;

                return null;
            }
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
                rootFrame.Navigate(typeof(MainPage), e.Arguments);

            Window.Current.Activate();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
