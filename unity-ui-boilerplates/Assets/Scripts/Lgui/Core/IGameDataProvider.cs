using System;
using UnityEngine;

namespace Lgui.Examples
{
    /// <summary>
    /// 게임 데이터(재화, 프로필 등)를 제공하는 인터페이스입니다.
    /// 실제 서버 데이터나 Mock 데이터로 교체 가능합니다.
    /// </summary>
    public interface IGameDataProvider
    {
        int Gold { get; }
        int Diamond { get; }
        string UserName { get; }
        Sprite ProfileImage { get; }
        
        event Action OnDataChanged;
        void AddGold(int amount);
    }

    /// <summary>
    /// 개발 중 데이터 없이 UI만 확인하기 위한 Mock 데이터 제공자입니다.
    /// </summary>
    public class MockGameData : IGameDataProvider
    {
        public int Gold => 5000;
        public int Diamond => 150;
        public string UserName => "MockPlayer_001";
        public Sprite ProfileImage => null;

        public event Action OnDataChanged;
        public void AddGold(int amount) => Debug.Log($"Mock: Add {amount} Gold");
    }
}
