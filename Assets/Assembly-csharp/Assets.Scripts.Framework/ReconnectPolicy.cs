using Apollo;
using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class ReconnectPolicy
	{
		private BaseConnector connector;

		private tryReconnectDelegate callback;

		private bool sessionStopped;

		private float reconnectTime;

		private uint reconnectCount = 4u;

		private uint tryCount;

		private uint connectTimeout = 10u;

		public bool shouldReconnect;

		public void SetConnector(BaseConnector inConnector, tryReconnectDelegate inEvent, uint tryMax)
		{
			this.StopPolicy();
			this.connector = inConnector;
			this.callback = inEvent;
			this.reconnectCount = tryMax;
		}

		public void StopPolicy()
		{
			this.sessionStopped = false;
			this.shouldReconnect = false;
			this.reconnectTime = this.connectTimeout;
			this.tryCount = 0u;
		}

		public void StartPolicy(ApolloResult result, int timeWait)
		{
			switch (result)
			{
			case ApolloResult.Success:
				this.shouldReconnect = false;
				this.sessionStopped = false;
				return;
			case ApolloResult.Error:
				IL_18:
				switch (result)
				{
				case ApolloResult.GcpError:
				case ApolloResult.PeerStopSession:
					goto IL_81;
				case ApolloResult.PeerCloseConnection:
					IL_2D:
					if (result != ApolloResult.ConnectFailed && result != ApolloResult.TokenSvrError)
					{
						this.shouldReconnect = true;
						this.sessionStopped = true;
						this.reconnectTime = (float)((this.tryCount != 0u) ? timeWait : 0);
						return;
					}
					goto IL_81;
				}
				goto IL_2D;
			case ApolloResult.NetworkException:
				this.shouldReconnect = true;
				this.sessionStopped = false;
				this.reconnectTime = (float)((this.tryCount != 0u) ? timeWait : 0);
				return;
			case ApolloResult.Timeout:
				goto IL_81;
			}
			goto IL_18;
			IL_81:
			this.shouldReconnect = true;
			this.sessionStopped = true;
			this.reconnectTime = (float)((this.tryCount != 0u) ? timeWait : 0);
		}

		public void UpdatePolicy(bool bForce)
		{
			if (this.connector != null && !this.connector.connected)
			{
				if (bForce)
				{
					this.reconnectTime = this.connectTimeout;
					this.tryCount = this.reconnectCount;
					if (this.sessionStopped)
					{
						this.connector.RestartConnector();
					}
					else
					{
						this.connector.RestartConnector();
					}
				}
				else
				{
					this.reconnectTime -= Time.unscaledDeltaTime;
					if (this.reconnectTime < 0f)
					{
						this.tryCount += 1u;
						this.reconnectTime = this.connectTimeout;
						uint num = this.tryCount;
						if (this.callback != null)
						{
							num = this.callback(num, this.reconnectCount);
						}
						if (num > this.reconnectCount)
						{
							return;
						}
						this.tryCount = num;
						if (this.sessionStopped)
						{
							this.connector.RestartConnector();
						}
						else
						{
							this.connector.RestartConnector();
						}
					}
				}
			}
		}
	}
}
