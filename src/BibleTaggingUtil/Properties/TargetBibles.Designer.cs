﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BibleTaggingUtil.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.8.0.0")]
    internal sealed partial class TargetBibles : global::System.Configuration.ApplicationSettingsBase {
        
        private static TargetBibles defaultInstance = ((TargetBibles)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new TargetBibles())));
        
        public static TargetBibles Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TargetBiblesFolder {
            get {
                return ((string)(this["TargetBiblesFolder"]));
            }
            set {
                this["TargetBiblesFolder"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TargetBible {
            get {
                return ((string)(this["TargetBible"]));
            }
            set {
                this["TargetBible"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RightToLeft {
            get {
                return ((bool)(this["RightToLeft"]));
            }
            set {
                this["RightToLeft"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Versification {
            get {
                return ((string)(this["Versification"]));
            }
            set {
                this["Versification"] = value;
            }
        }
    }
}