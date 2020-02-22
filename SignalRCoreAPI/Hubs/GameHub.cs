using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRCoreAPI.Hubs {
    [Serializable]
    public enum MoveType {
        One,
        Two,
        Three
    }

    [Serializable]
    public class Move {
        public int MoveNumber;
        public DateTimeOffset Time;
        public MoveType MoveType;
        public Dictionary<string, object> Data;
    }

    public class GameHub : Hub {
        #region Core

        public override async Task OnConnectedAsync () {
            await Clients.All.SendAsync ("ReceiveMessage", $"{Context.ConnectionId} joined");
        }

        public Task Send (string message) {
            return Clients.All.SendAsync ("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }

        public override async Task OnDisconnectedAsync (Exception ex) {
            await Clients.Others.SendAsync ("ReceiveMessage", $"{Context.ConnectionId} left");
        }

        public Task SendToOthers (string message) {
            return Clients.Others.SendAsync ("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }

        public Task SendToConnection (string connectionId, string message) {
            return Clients.Client (connectionId).SendAsync ("ReceiveMessage", $"Private message from {Context.ConnectionId}: {message}");
        }

        public Task SendToGroup (string groupName, string message) {
            return Clients.Group (groupName).SendAsync ("ReceiveMessage", $"{Context.ConnectionId}@{groupName}: {message}");
        }

        public Task SendToOthersInGroup (string groupName, string message) {
            return Clients.OthersInGroup (groupName).SendAsync ("ReceiveMessage", $"{Context.ConnectionId}@{groupName}: {message}");
        }

        public async Task JoinGroup (string groupName) {
            await Groups.AddToGroupAsync (Context.ConnectionId, groupName);

            await Clients.Group (groupName).SendAsync ("ReceiveMessage", $"{Context.ConnectionId} joined {groupName}");
        }

        public async Task LeaveGroup (string groupName) {
            await Clients.Group (groupName).SendAsync ("ReceiveMessage", $"{Context.ConnectionId} left {groupName}");

            await Groups.RemoveFromGroupAsync (Context.ConnectionId, groupName);
        }

        #endregion

        public async Task SendMessage (string message) {
            await Clients.All.SendAsync ("ReceiveMessage", message);
        }

        public async Task SendDictionary (Dictionary<string, object> data, DateTimeOffset when) {
            await Clients.Client (Context.ConnectionId).SendAsync ("ReceiveDictionary", data, when);
        }

        public async Task SendMove (Move move) {
            await Clients.Client (Context.ConnectionId).SendAsync ("ReceiveMove", move);
        }

        public async Task SendMoves (List<Move> moves) {
            await Clients.Client (Context.ConnectionId).SendAsync ("ReceiveMoves", moves);
        }
    }
}