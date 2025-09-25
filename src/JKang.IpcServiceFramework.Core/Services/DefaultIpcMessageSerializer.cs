using System;
using System.Text;
#if NET8_0_OR_GREATER
using System.Text.Json;
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace JKang.IpcServiceFramework.Services
{
    public class DefaultIpcMessageSerializer : IIpcMessageSerializer
    {
#if NET8_0_OR_GREATER
        private static readonly JsonSerializerOptions _settings = new JsonSerializerOptions
        {

        };
#else
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects
        };
#endif



        public IpcRequest DeserializeRequest(byte[] binary)
        {
            return Deserialize<IpcRequest>(binary);
        }

        public IpcResponse DeserializeResponse(byte[] binary)
        {
            return Deserialize<IpcResponse>(binary);
        }

        public byte[] SerializeRequest(IpcRequest request)
        {
            return Serialize(request);
        }

        public byte[] SerializeResponse(IpcResponse response)
        {
            return Serialize(response);
        }

        private T Deserialize<T>(byte[] binary)
        {
            try
            {
#if NET8_0_OR_GREATER
                return JsonSerializer.Deserialize<T>(binary, _settings);
#else                
                string json = Encoding.UTF8.GetString(binary);
                return JsonConvert.DeserializeObject<T>(json, _settings);
#endif
            }
            catch (Exception ex) when (
#if NET8_0_OR_GREATER
#else
    ex is JsonSerializationException ||
#endif
            ex is ArgumentException ||
            ex is EncoderFallbackException)
            {
                throw new IpcSerializationException("Failed to deserialize IPC message", ex);
            }
        }

        private byte[] Serialize(object obj)
        {
            try
            {
#if NET8_0_OR_GREATER
                using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
                {
                    JsonSerializer.Serialize(stream, _settings);
                    return stream.ToArray();
                }
#else
                string json = JsonConvert.SerializeObject(obj, _settings);
                  return Encoding.UTF8.GetBytes(json);
#endif
            }
            catch (Exception ex) when (
#if NET8_0_OR_GREATER
#else
    ex is JsonSerializationException ||
#endif
                ex is EncoderFallbackException)
            {
                throw new IpcSerializationException("Failed to serialize IPC message", ex);
            }
        }
    }
}
