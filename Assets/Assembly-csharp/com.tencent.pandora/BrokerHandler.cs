using com.tencent.pandora.MiniJSON;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace com.tencent.pandora
{
	public class BrokerHandler : TCPSocketHandler
	{
		private Action<int, Dictionary<string, object>> statusChangedAction;

		private Action<int, Dictionary<string, object>> packetRecvdAction;

		public BrokerHandler(Action<int, Dictionary<string, object>> statusChangedAction, Action<int, Dictionary<string, object>> packetRecvdAction)
		{
			this.statusChangedAction = statusChangedAction;
			this.packetRecvdAction = packetRecvdAction;
		}

		public override void OnConnected()
		{
			Logger.DEBUG(string.Empty);
			Message message = new Message();
			message.status = 0;
			message.content.set_Item("uniqueSocketId", base.GetUniqueSocketId());
			message.action = this.statusChangedAction;
			Pandora.Instance.GetNetLogic().EnqueueResult(message);
		}

		public override int DetectPacketSize(byte[] receivedData, int dataLen)
		{
			Logger.DEBUG(string.Empty);
			if (dataLen < 4)
			{
				return 0;
			}
			uint num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedData, 0));
			return (int)(num + 4u);
		}

		public override void OnReceived(Packet thePacket)
		{
			Logger.DEBUG(string.Empty);
			try
			{
				string text = Convert.ToBase64String(thePacket.theContent, 4, thePacket.theContent.Length - 4);
				string text2 = MinizLib.UnCompress(text.get_Length(), text);
				byte[] array = Convert.FromBase64String(text2);
				string @string = Encoding.get_UTF8().GetString(array);
				Logger.DEBUG(@string);
				Message message = new Message();
				message.status = 0;
				message.content = (Json.Deserialize(@string) as Dictionary<string, object>);
				message.action = this.packetRecvdAction;
				Pandora.Instance.GetNetLogic().EnqueueResult(message);
			}
			catch (Exception ex)
			{
				Logger.ERROR(ex.get_StackTrace());
			}
		}

		public override void OnClose()
		{
			Logger.DEBUG(string.Empty);
			Message message = new Message();
			message.status = -1;
			message.content.set_Item("uniqueSocketId", base.GetUniqueSocketId());
			message.action = this.statusChangedAction;
			Pandora.Instance.GetNetLogic().EnqueueResult(message);
		}
	}
}
