using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using NHibernate.Validator.Engine;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace HMV.Core.Framework.Exception
{
    public class GenericErrorDescription
    {
        public string Message { get; set; }
    }

    public class JsonErrorHandler : IEndpointBehavior, IErrorHandler
    {
        public bool HandleError(System.Exception error)
        {
            return true;
        }

        public void ProvideFault(System.Exception error, MessageVersion version, ref Message fault)
        {
            GenericErrorDescription desc =
                new GenericErrorDescription() { Message = error.Message };

            fault = Message.CreateMessage(version, "", desc,
                new DataContractJsonSerializer(typeof(GenericErrorDescription)));
            fault.Properties.Add(WebBodyFormatMessageProperty.Name,
                new WebBodyFormatMessageProperty(WebContentFormat.Json));

            var msgp = new HttpResponseMessageProperty();
            msgp.Headers[HttpResponseHeader.ContentType] = "application/json";
            msgp.StatusCode = HttpStatusCode.InternalServerError;
            msgp.StatusDescription = desc.Message;

            fault.Properties.Add(HttpResponseMessageProperty.Name, msgp);
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bp) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime cr) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher ed)
        {
            ed.ChannelDispatcher.ErrorHandlers.Add(this);
        }

        public void Validate(ServiceEndpoint endpoint) { }

    }
    
    public class JsonErrorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get
            {
                return typeof(JsonErrorHandler);
            }
        }

        protected override object CreateBehavior()
        {
            return new JsonErrorHandler();
        }
    }
}
