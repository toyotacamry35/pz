using System;
using PzLauncher.Models.Dto;

namespace Uins
{
    public struct FriendInfo
    {
        public Guid UserId;
        public string Login;
        public FriendStatus Status;

        public FriendInfo(Friend friend)
        {
            UserId = Guid.Parse(friend.UserId);
            Login = friend.Login;
            Status = friend.Status == "online"
                ? FriendStatus.Online
                : friend.Status == "booked"
                    ? FriendStatus.Booked
                    : FriendStatus.Offline;
        }

        public override string ToString()
        {
            return $"FriendInfo(Login='{Login}', Status={Status}, UserId={UserId})";
        }
    }

    public enum FriendStatus
    {
        Offline = 0,
        Booked = 1,
        Online = 2,
    }
}