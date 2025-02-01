using OctoMessager.Client.Configuration;
using OctoMessager.Client.Models.Webhooks;
using System.Text.Json;

namespace OctoMessager.Client.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            OctoClient client = new(new HttpClient(), new OctoClientOptions() {ApiKey = ""});
            IWebhookEvent? result = client.ParseEvent("""
                {
                    "event": "v1/messageReceived",
                    "data": {
                        "text": "Hello, there!",
                        "messageInfo": {
                            "fromSelf": false,
                            "groupID": null,
                            "messageID": "0123456789ABCDEF",
                            "timestamp": 1738396620,
                            "user": {
                                "id": "0123456789@s.whatsapp.net",
                                "profileUrl": "https://example.com",
                                "pushName": "Example usr"
                            }
                        },
                        "messageType": "text",
                        "mimeType": "text/plain; charset=utf-8",
                        "quoted": null
                    }
                }
                """);
            Assert.NotNull(result);
            if(result != null)
            {
                Assert.IsType<V1MessageReceived>(result);
                Assert.Equal(WebhookEventType.MessageReceived, result.EventType);
                Assert.Equal("Hello, there!", ((IMessage)result).Text);
            }
        }
    }
}