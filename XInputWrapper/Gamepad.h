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

#pragma once

#include <XInput.h>
using namespace Platform;

namespace XInputWrapper
{
    public value struct GamepadData sealed
    {
        bool    ControllerConnected;

        float   LeftAngle;
        float   LeftIntensity;

        float   RightAngle;
        float   RightIntensity;

        float   LeftTrigger;
        float   RightTrigger;

        bool    ButtonA;
        bool    ButtonB;
        bool    ButtonX;
        bool    ButtonY;
    };

    public delegate void SomethingHappened(String^ message);

    public ref class Gamepad sealed
    {
    public:
        Gamepad();

    private: // Fields
        bool                    _isControllerConnected;  
        XINPUT_CAPABILITIES     _currentCapabilities;             
        XINPUT_STATE            _currentState;
        uint64                  _lastEnumTime = GetTickCount64();
        uint64                  _ConnectScanTimeout = 2000; 

    public: // External methods
        GamepadData Poll();

    private: // Internal methods
        void UpdateConnection();
    };
}