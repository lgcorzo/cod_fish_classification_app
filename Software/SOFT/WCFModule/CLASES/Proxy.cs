﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión del motor en tiempo de ejecución:2.0.50727.8689
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServiceLibraryClient
{
    using System.Runtime.Serialization;


    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "DataContract1", Namespace = "http://schemas.datacontract.org/2004/07/ServiceLibrary")]
    public partial class DataContract1 : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private string FirstNameField;

        private ServiceLibrary.ImagesToProcessRemote ImagenesRemoteField;

        private string LastNameField;

        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName
        {
            get
            {
                return this.FirstNameField;
            }
            set
            {
                this.FirstNameField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceLibrary.ImagesToProcessRemote ImagenesRemote
        {
            get
            {
                return this.ImagenesRemoteField;
            }
            set
            {
                this.ImagenesRemoteField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName
        {
            get
            {
                return this.LastNameField;
            }
            set
            {
                this.LastNameField = value;
            }
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ImagesToProcessRemote", Namespace = "http://schemas.datacontract.org/2004/07/ServiceLibrary")]
    public partial class ImagesToProcessRemote : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private int IDproductoField;

        private int IDproductoCSRField;

        private int LineaField;

        private string Nombre_variableField;

        private System.DateTime TimeStampField;

        private HalconDotNet.HImage img1NIRField;

        private HalconDotNet.HImage img1RGBField;

        private HalconDotNet.HImage img2NIRField;

        private HalconDotNet.HImage img2RGBField;

        private HalconDotNet.HImage img3NIRField;

        private HalconDotNet.HImage img3RGBField;

        private HalconDotNet.HImage imgAnteriorRGBField;

        private HalconDotNet.HImage imgPosteriorRGBField;
        
        private ServiceLibrary.Parametros_Process parametrosField;
        
        private int taxiField;

        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IDproducto
        {
            get
            {
                return this.IDproductoField;
            }
            set
            {
                this.IDproductoField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IDproductoCSR
        {
            get
            {
                return this.IDproductoCSRField;
            }
            set
            {
                this.IDproductoCSRField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Linea
        {
            get
            {
                return this.LineaField;
            }
            set
            {
                this.LineaField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Nombre_variable
        {
            get
            {
                return this.Nombre_variableField;
            }
            set
            {
                this.Nombre_variableField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime TimeStamp
        {
            get
            {
                return this.TimeStampField;
            }
            set
            {
                this.TimeStampField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage img1NIR
        {
            get
            {
                return this.img1NIRField;
            }
            set
            {
                this.img1NIRField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage img1RGB
        {
            get
            {
                return this.img1RGBField;
            }
            set
            {
                this.img1RGBField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage img2NIR
        {
            get
            {
                return this.img2NIRField;
            }
            set
            {
                this.img2NIRField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage img2RGB
        {
            get
            {
                return this.img2RGBField;
            }
            set
            {
                this.img2RGBField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage img3NIR
        {
            get
            {
                return this.img3NIRField;
            }
            set
            {
                this.img3NIRField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage img3RGB
        {
            get
            {
                return this.img3RGBField;
            }
            set
            {
                this.img3RGBField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage imgAnteriorRGB
        {
            get
            {
                return this.imgAnteriorRGBField;
            }
            set
            {
                this.imgAnteriorRGBField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public HalconDotNet.HImage imgPosteriorRGB
        {
            get
            {
                return this.imgPosteriorRGBField;
            }
            set
            {
                this.imgPosteriorRGBField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ServiceLibrary.Parametros_Process parametros
        {
            get
            {
                return this.parametrosField;
            }
            set
            {
                this.parametrosField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int taxi
        {
            get
            {
                return this.taxiField;
            }
            set
            {
                this.taxiField = value;
            }
        }
    }
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Parametros_Process", Namespace = "http://schemas.datacontract.org/2004/07/ServiceLibrary")]
    public partial class Parametros_Process : object, System.Runtime.Serialization.IExtensibleDataObject
    {

        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        private float fParam1Field;

        private float fParam2Field;

        private float fParam3Field;

        private float fParam4Field;

        private int iParam1Field;

        private int iParam2Field;

        private int iParam3Field;

        private int iParam4Field;

        private string sParam1Field;

        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public float fParam1
        {
            get
            {
                return this.fParam1Field;
            }
            set
            {
                this.fParam1Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public float fParam2
        {
            get
            {
                return this.fParam2Field;
            }
            set
            {
                this.fParam2Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public float fParam3
        {
            get
            {
                return this.fParam3Field;
            }
            set
            {
                this.fParam3Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public float fParam4
        {
            get
            {
                return this.fParam4Field;
            }
            set
            {
                this.fParam4Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int iParam1
        {
            get
            {
                return this.iParam1Field;
            }
            set
            {
                this.iParam1Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int iParam2
        {
            get
            {
                return this.iParam2Field;
            }
            set
            {
                this.iParam2Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int iParam3
        {
            get
            {
                return this.iParam3Field;
            }
            set
            {
                this.iParam3Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int iParam4
        {
            get
            {
                return this.iParam4Field;
            }
            set
            {
                this.iParam4Field = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string sParam1
        {
            get
            {
                return this.sParam1Field;
            }
            set
            {
                this.sParam1Field = value;
            }
        }
    }




    namespace HalconDotNet
    {
        using System;
        using System.Runtime.Serialization;


        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
        [System.SerializableAttribute()]
        public partial class HImage : HalconDotNet.HObject
        {

            public HImage(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) :
                    base(info, context)
            {
            }
        }

        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
        [System.SerializableAttribute()]
        [System.Runtime.Serialization.KnownTypeAttribute(typeof(HalconDotNet.HImage))]
        public partial class HObject : object, System.Runtime.Serialization.ISerializable
        {

            private System.Runtime.Serialization.SerializationInfo info;

            public HObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                this.info = info;
            }

            public System.Runtime.Serialization.SerializationInfo SerializationInfo
            {
                get
                {
                    return this.info;
                }
                set
                {
                    this.info = value;
                }
            }

            public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            {
                if ((this.SerializationInfo == null))
                {
                    return;
                }
                System.Runtime.Serialization.SerializationInfoEnumerator enumerator = this.SerializationInfo.GetEnumerator();
                for (
                ; enumerator.MoveNext();
                )
                {
                    System.Runtime.Serialization.SerializationEntry entry = enumerator.Current;
                    info.AddValue(entry.Name, entry.Value);
                }
            }
        }
    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "IService1")]
    public interface IService1
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IService1/MyOperation1", ReplyAction = "http://tempuri.org/IService1/MyOperation1Response")]
        string MyOperation1(string myValue);

        [System.ServiceModel.OperationContractAttribute(IsOneWay = true, Action = "http://tempuri.org/IService1/MyOperation2")]
        void MyOperation2(ServiceLibraryClient.DataContract1 dataContractValue);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IService1/HelloWorld", ReplyAction = "http://tempuri.org/IService1/HelloWorldResponse")]
        string HelloWorld(string str);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public interface IService1Channel : IService1, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
    public partial class Service1Client : System.ServiceModel.ClientBase<IService1>, IService1
    {

        public Service1Client()
        {
        }

        public Service1Client(string endpointConfigurationName) :
                base(endpointConfigurationName)
        {
        }

        public Service1Client(string endpointConfigurationName, string remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public Service1Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
                base(endpointConfigurationName, remoteAddress)
        {
        }

        public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public string MyOperation1(string myValue)
        {
            return base.Channel.MyOperation1(myValue);
        }

        public void MyOperation2(ServiceLibraryClient.DataContract1 dataContractValue)
        {
            base.Channel.MyOperation2(dataContractValue);
        }

        public string HelloWorld(string str)
        {
            return base.Channel.HelloWorld(str);
        }
    }
}