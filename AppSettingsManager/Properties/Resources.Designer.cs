﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppSettingsManager.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AppSettingsManager.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на NjAzNTk2NzI0ODI4NzY2MjEx.XURPVA.hVSFd4AwXXzWqbaO5gl5Z1dNJmQ.
        /// </summary>
        internal static string BOT_TOKEN {
            get {
                return ResourceManager.GetString("BOT_TOKEN", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 574251966440407043.
        /// </summary>
        internal static string CHANNEL_ID {
            get {
                return ResourceManager.GetString("CHANNEL_ID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\1\Desktop\Reps\ArtificialisExcogitatoris\SentenceFinisher\Data\BagOfWords.mdf;Integrated Security=True.
        /// </summary>
        internal static string DEFAULT_CONNECTION_STRING {
            get {
                return ResourceManager.GetString("DEFAULT_CONNECTION_STRING", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на 488731299406938132.
        /// </summary>
        internal static string SERVER_ID {
            get {
                return ResourceManager.GetString("SERVER_ID", resourceCulture);
            }
        }
    }
}
