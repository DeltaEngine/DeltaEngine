﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DeltaEngine.Logging.Tests.LogService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://log.deltaengine.net/", ConfigurationName="LogService.LogServiceSoap")]
    public interface LogServiceSoap {
        
        // CODEGEN: Generating message contract since element name messageType from namespace http://log.deltaengine.net/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://log.deltaengine.net/Log", ReplyAction="*")]
        DeltaEngine.Logging.Tests.LogService.LogResponse Log(DeltaEngine.Logging.Tests.LogService.LogRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Log", Namespace="http://log.deltaengine.net/", Order=0)]
        public DeltaEngine.Logging.Tests.LogService.LogRequestBody Body;
        
        public LogRequest() {
        }
        
        public LogRequest(DeltaEngine.Logging.Tests.LogService.LogRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://log.deltaengine.net/")]
    public partial class LogRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string messageType;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string message;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string projectName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string apiKey;
        
        public LogRequestBody() {
        }
        
        public LogRequestBody(string messageType, string message, string projectName, string apiKey) {
            this.messageType = messageType;
            this.message = message;
            this.projectName = projectName;
            this.apiKey = apiKey;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class LogResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="LogResponse", Namespace="http://log.deltaengine.net/", Order=0)]
        public DeltaEngine.Logging.Tests.LogService.LogResponseBody Body;
        
        public LogResponse() {
        }
        
        public LogResponse(DeltaEngine.Logging.Tests.LogService.LogResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class LogResponseBody {
        
        public LogResponseBody() {
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LogServiceSoapChannel : DeltaEngine.Logging.Tests.LogService.LogServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LogServiceSoapClient : System.ServiceModel.ClientBase<DeltaEngine.Logging.Tests.LogService.LogServiceSoap>, DeltaEngine.Logging.Tests.LogService.LogServiceSoap {
        
        public LogServiceSoapClient() {
        }
        
        public LogServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LogServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LogServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LogServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        DeltaEngine.Logging.Tests.LogService.LogResponse DeltaEngine.Logging.Tests.LogService.LogServiceSoap.Log(DeltaEngine.Logging.Tests.LogService.LogRequest request) {
            return base.Channel.Log(request);
        }
        
        public void Log(string messageType, string message, string projectName, string apiKey) {
            DeltaEngine.Logging.Tests.LogService.LogRequest inValue = new DeltaEngine.Logging.Tests.LogService.LogRequest();
            inValue.Body = new DeltaEngine.Logging.Tests.LogService.LogRequestBody();
            inValue.Body.messageType = messageType;
            inValue.Body.message = message;
            inValue.Body.projectName = projectName;
            inValue.Body.apiKey = apiKey;
            DeltaEngine.Logging.Tests.LogService.LogResponse retVal = ((DeltaEngine.Logging.Tests.LogService.LogServiceSoap)(this)).Log(inValue);
        }
    }
}
