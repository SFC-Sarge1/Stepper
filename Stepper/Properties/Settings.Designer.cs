// ***********************************************************************
// Assembly         : Stepper
// Author           : sfcsarge
// Created          : 04-01-2024
//
// Last Modified By : sfcsarge
// Last Modified On : 07-24-2024
// ***********************************************************************
// <copyright file="Settings.Designer.cs" company="Stepper">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Stepper.Properties {


    /// <summary>
    /// Class Settings. This class cannot be inherited.
    /// Implements the <see cref="System.Configuration.ApplicationSettingsBase" />
    /// </summary>
    /// <seealso cref="System.Configuration.ApplicationSettingsBase" />
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.10.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {

        /// <summary>
        /// The default instance
        /// </summary>
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));

        /// <summary>
        /// Gets the default.
        /// </summary>
        /// <value>The default.</value>
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [CKB xaxis reset to zero is checked].
        /// </summary>
        /// <value><c>true</c> if [CKB xaxis reset to zero is checked]; otherwise, <c>false</c>.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ckbXaxisResetToZeroIsChecked {
            get {
                return ((bool)(this["ckbXaxisResetToZeroIsChecked"]));
            }
            set {
                this["ckbXaxisResetToZeroIsChecked"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [CKB yaxis reset to zero is checked].
        /// </summary>
        /// <value><c>true</c> if [CKB yaxis reset to zero is checked]; otherwise, <c>false</c>.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ckbYaxisResetToZeroIsChecked {
            get {
                return ((bool)(this["ckbYaxisResetToZeroIsChecked"]));
            }
            set {
                this["ckbYaxisResetToZeroIsChecked"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [CKB zaxis reset to zero is checked].
        /// </summary>
        /// <value><c>true</c> if [CKB zaxis reset to zero is checked]; otherwise, <c>false</c>.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ckbZaxisResetToZeroIsChecked {
            get {
                return ((bool)(this["ckbZaxisResetToZeroIsChecked"]));
            }
            set {
                this["ckbZaxisResetToZeroIsChecked"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the divider4 00.
        /// </summary>
        /// <value>The divider4 00.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4.00")]
        public decimal Divider4_00 {
            get {
                return ((decimal)(this["Divider4_00"]));
            }
            set {
                this["Divider4_00"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the milliseconds.
        /// </summary>
        /// <value>The milliseconds.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000")]
        public int Milliseconds {
            get {
                return ((int)(this["Milliseconds"]));
            }
            set {
                this["Milliseconds"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the mm per revolution.
        /// </summary>
        /// <value>The mm per revolution.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("4")]
        public int mmPerRevolution {
            get {
                return ((int)(this["mmPerRevolution"]));
            }
            set {
                this["mmPerRevolution"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the root axis x.
        /// </summary>
        /// <value>The root axis x.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("X")]
        public string RootAxisX {
            get {
                return ((string)(this["RootAxisX"]));
            }
            set {
                this["RootAxisX"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the root axis y.
        /// </summary>
        /// <value>The root axis y.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Y")]
        public string RootAxisY {
            get {
                return ((string)(this["RootAxisY"]));
            }
            set {
                this["RootAxisY"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the root axis z.
        /// </summary>
        /// <value>The root axis z.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Z")]
        public string RootAxisZ {
            get {
                return ((string)(this["RootAxisZ"]));
            }
            set {
                this["RootAxisZ"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the root axis xy.
        /// </summary>
        /// <value>The root axis xy.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("XY")]
        public string RootAxisXY {
            get {
                return ((string)(this["RootAxisXY"]));
            }
            set {
                this["RootAxisXY"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value 0 00.
        /// </summary>
        /// <value>The value 0 00.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal Value_0_00 {
            get {
                return ((decimal)(this["Value_0_00"]));
            }
            set {
                this["Value_0_00"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [xaxis changed].
        /// </summary>
        /// <value><c>true</c> if [xaxis changed]; otherwise, <c>false</c>.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool XaxisChanged {
            get {
                return ((bool)(this["XaxisChanged"]));
            }
            set {
                this["XaxisChanged"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the xaxis stepper current.
        /// </summary>
        /// <value>The xaxis stepper current.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal XaxisStepperCurrent {
            get {
                return ((decimal)(this["XaxisStepperCurrent"]));
            }
            set {
                this["XaxisStepperCurrent"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the xaxis stepper move.
        /// </summary>
        /// <value>The xaxis stepper move.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal XaxisStepperMove {
            get {
                return ((decimal)(this["XaxisStepperMove"]));
            }
            set {
                this["XaxisStepperMove"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the xaxis motor speed.
        /// </summary>
        /// <value>The xaxis motor speed.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("400.00")]
        public decimal XaxisMotorSpeed {
            get {
                return ((decimal)(this["XaxisMotorSpeed"]));
            }
            set {
                this["XaxisMotorSpeed"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [yaxis changed].
        /// </summary>
        /// <value><c>true</c> if [yaxis changed]; otherwise, <c>false</c>.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool YaxisChanged {
            get {
                return ((bool)(this["YaxisChanged"]));
            }
            set {
                this["YaxisChanged"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the yaxis stepper current.
        /// </summary>
        /// <value>The yaxis stepper current.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal YaxisStepperCurrent {
            get {
                return ((decimal)(this["YaxisStepperCurrent"]));
            }
            set {
                this["YaxisStepperCurrent"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the yaxis stepper move.
        /// </summary>
        /// <value>The yaxis stepper move.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal YaxisStepperMove {
            get {
                return ((decimal)(this["YaxisStepperMove"]));
            }
            set {
                this["YaxisStepperMove"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the yaxis motor speed.
        /// </summary>
        /// <value>The yaxis motor speed.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("400.00")]
        public decimal YaxisMotorSpeed {
            get {
                return ((decimal)(this["YaxisMotorSpeed"]));
            }
            set {
                this["YaxisMotorSpeed"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [zaxis changed].
        /// </summary>
        /// <value><c>true</c> if [zaxis changed]; otherwise, <c>false</c>.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool ZaxisChanged {
            get {
                return ((bool)(this["ZaxisChanged"]));
            }
            set {
                this["ZaxisChanged"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the zaxis stepper current.
        /// </summary>
        /// <value>The zaxis stepper current.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal ZaxisStepperCurrent {
            get {
                return ((decimal)(this["ZaxisStepperCurrent"]));
            }
            set {
                this["ZaxisStepperCurrent"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the zaxis stepper move.
        /// </summary>
        /// <value>The zaxis stepper move.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal ZaxisStepperMove {
            get {
                return ((decimal)(this["ZaxisStepperMove"]));
            }
            set {
                this["ZaxisStepperMove"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the zaxis motor speed.
        /// </summary>
        /// <value>The zaxis motor speed.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("400.00")]
        public decimal ZaxisMotorSpeed {
            get {
                return ((decimal)(this["ZaxisMotorSpeed"]));
            }
            set {
                this["ZaxisMotorSpeed"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the zero xaxis.
        /// </summary>
        /// <value>The zero xaxis.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int zeroXaxis {
            get {
                return ((int)(this["zeroXaxis"]));
            }
            set {
                this["zeroXaxis"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the zero yaxis.
        /// </summary>
        /// <value>The zero yaxis.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int zeroYaxis {
            get {
                return ((int)(this["zeroYaxis"]));
            }
            set {
                this["zeroYaxis"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the zero zaxis.
        /// </summary>
        /// <value>The zero zaxis.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int zeroZaxis {
            get {
                return ((int)(this["zeroZaxis"]));
            }
            set {
                this["zeroZaxis"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the milisecond timer interval.
        /// </summary>
        /// <value>The milisecond timer interval.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.00")]
        public decimal MilisecondTimerInterval {
            get {
                return ((decimal)(this["MilisecondTimerInterval"]));
            }
            set {
                this["MilisecondTimerInterval"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the millisecond delay.
        /// </summary>
        /// <value>The millisecond delay.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1000.00")]
        public decimal MillisecondDelay {
            get {
                return ((decimal)(this["MillisecondDelay"]));
            }
            set {
                this["MillisecondDelay"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the math part2 initlz.
        /// </summary>
        /// <value>The math part2 initlz.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.00")]
        public decimal MathPart2Initlz {
            get {
                return ((decimal)(this["MathPart2Initlz"]));
            }
            set {
                this["MathPart2Initlz"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the build version.
        /// </summary>
        /// <value>The build version.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1.00")]
        public string BuildVersion {
            get {
                return ((string)(this["BuildVersion"]));
            }
            set {
                this["BuildVersion"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the project file path.
        /// </summary>
        /// <value>The project file path.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\\\Users\\\\sfcsarge\\\\source\\\\repos\\\\Stepper\\\\Stepper\\\\Stepper.csproj")]
        public string ProjectFilePath {
            get {
                return ((string)(this["ProjectFilePath"]));
            }
            set {
                this["ProjectFilePath"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the count down text.
        /// </summary>
        /// <value>The count down text.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Stepper Motor Timer:")]
        public string CountDownText {
            get {
                return ((string)(this["CountDownText"]));
            }
            set {
                this["CountDownText"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the server ip address.
        /// </summary>
        /// <value>The server ip address.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.4.25")]
        public string ServerIPAddress {
            get {
                return ((string)(this["ServerIPAddress"]));
            }
            set {
                this["ServerIPAddress"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the baud rate.
        /// </summary>
        /// <value>The baud rate.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9600")]
        public int BaudRate {
            get {
                return ((int)(this["BaudRate"]));
            }
            set {
                this["BaudRate"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the COM port.
        /// </summary>
        /// <value>The COM port.</value>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("COM10")]
        public string ComPort {
            get {
                return ((string)(this["ComPort"]));
            }
            set {
                this["ComPort"] = value;
            }
        }
    }
}
