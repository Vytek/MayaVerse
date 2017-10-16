// automatically generated, do not modify

namespace HazelMessage
{

using FlatBuffers;

public sealed class HMessage : Table {
  public static HMessage GetRootAsHMessage(ByteBuffer _bb) { return GetRootAsHMessage(_bb, new HMessage()); }
  public static HMessage GetRootAsHMessage(ByteBuffer _bb, HMessage obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public HMessage __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public sbyte Command { get { int o = __offset(4); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public string Answer { get { int o = __offset(6); return o != 0 ? __string(o + bb_pos) : null; } }

  public static Offset<HMessage> CreateHMessage(FlatBufferBuilder builder,
      sbyte Command = 0,
      StringOffset Answer = default(StringOffset)) {
    builder.StartObject(2);
    HMessage.AddAnswer(builder, Answer);
    HMessage.AddCommand(builder, Command);
    return HMessage.EndHMessage(builder);
  }

  public static void StartHMessage(FlatBufferBuilder builder) { builder.StartObject(2); }
  public static void AddCommand(FlatBufferBuilder builder, sbyte Command) { builder.AddSbyte(0, Command, 0); }
  public static void AddAnswer(FlatBufferBuilder builder, StringOffset AnswerOffset) { builder.AddOffset(1, AnswerOffset.Value, 0); }
  public static Offset<HMessage> EndHMessage(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<HMessage>(o);
  }
  public static void FinishHMessageBuffer(FlatBufferBuilder builder, Offset<HMessage> offset) { builder.Finish(offset.Value); }
};


}
