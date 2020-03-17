namespace ArtificialisExcogitatoris
{
  using Discord.WebSocket;
  using Google.Apis.Dialogflow.v2;
  using Google.Apis.Services;
  using System;

  public class DockflowWorker : IDisposable
  {
    private readonly string apiKey = "2ab8e598104e4112bb7755492da542f5";
    private DialogflowService service;

    public DockflowWorker()
    {
      this.service = new DialogflowService(new BaseClientService.Initializer
      {
        ApplicationName = "MelExDialogFlow",
        ApiKey = apiKey
      });
    }

    public void Dispose()
    {
      this.service.Dispose();
    }

    public string CreateAnswer(SocketMessage socketMessage)
    {
      /*var b = new Google.Apis.Dialogflow.v2.Data.GoogleCloudDialogflowV2Intent();
      b.Messages

      service.Projects.Agent.Intents.Create()*/


      return socketMessage.Content;
    }
  }
}
