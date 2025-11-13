# PingPong Server

## 제작 기간
2021.09.26 ~ 2021.11.11

## 개요

`PingPong Server`는 Akka.NET과 Redis를 활용한 실시간 채팅 서버입니다.

### 주요 기능
1. **TCP 소켓 기반 서버-클라이언트 통신**
2. **Redis Pub/Sub 기반 채널 채팅**
3. **리플렉션 기반 메시지 자동 라우팅 및 처리**

## 시스템 구조

```
pingpongserver/
├── Server/
│   ├── Server.cs              # TCP 서버 (포트: 63325)
│   └── PingPongConnection.cs  # 클라이언트 연결 액터
├── Message/
│   └── Message.cs             # 메시지 프로토콜 정의
├── Redis/
│   └── RedisConnection.cs     # Redis Pub/Sub 관리
├── System/
│   └── System.cs              # ActorSystem 싱글톤
└── Utils/
    └── Singleton.cs           # 싱글톤 베이스 클래스
```

---

## 메시지 프로토콜

```csharp
// 채널 입장/퇴장
EnterChatRoomReq  { ChannelName: string(16) }           // 요청
EnterChatRoomRes  { ChannelName: string(16), bool }     // 응답
LeaveChatRoom     { ChannelName: string(16) }           // 퇴장

// 메시지 송수신
SendChatting { ChannelName: string(16), ChatMessage: string(64) }  // 송신
RecvChatting { ChannelName: string(16), ChatMessage: string(64) }  // 수신
```

---

## TODO

- [ ] **Akka.Serialization으로 직렬화 방식 변경**
  - 현재 Marshal 기반 → Akka.Serialization으로 마이그레이션
  
- [ ] **패킷 처리 자동화 개선**
  - 리플렉션 최적화 (IL Emit / ExpressionTree 활용)
  - 메시지 팩토리 패턴 적용
  
- [ ] **에러 핸들링 체계 구축**
  - ErrorHandler, MessageErrorHandler 구현
  - 로깅 시스템 통합

---

## 참고 자료

- [Akka.NET I/O Documentation](https://getakka.net/articles/networking/io.html)
- [Akka.NET 한글 튜토리얼](https://blog.rajephon.dev/2018/12/08/akka-02/)
