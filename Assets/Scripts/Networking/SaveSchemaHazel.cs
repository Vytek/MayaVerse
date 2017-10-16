// automatically generated, do not modify

namespace HazelTest
{

using FlatBuffers;

public sealed class Vec3 : Struct {
  public Vec3 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }

  public static Offset<Vec3> CreateVec3(FlatBufferBuilder builder, float X, float Y, float Z) {
    builder.Prep(4, 12);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vec3>(builder.Offset);
  }
};

public sealed class Vec4 : Struct {
  public Vec4 __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public float X { get { return bb.GetFloat(bb_pos + 0); } }
  public float Y { get { return bb.GetFloat(bb_pos + 4); } }
  public float Z { get { return bb.GetFloat(bb_pos + 8); } }
  public float W { get { return bb.GetFloat(bb_pos + 12); } }

  public static Offset<Vec4> CreateVec4(FlatBufferBuilder builder, float X, float Y, float Z, float W) {
    builder.Prep(4, 16);
    builder.PutFloat(W);
    builder.PutFloat(Z);
    builder.PutFloat(Y);
    builder.PutFloat(X);
    return new Offset<Vec4>(builder.Offset);
  }
};

public sealed class Object : Table {
  public static Object GetRootAsObject(ByteBuffer _bb) { return GetRootAsObject(_bb, new Object()); }
  public static Object GetRootAsObject(ByteBuffer _bb, Object obj) { return (obj.__init(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public Object __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public sbyte Type { get { int o = __offset(4); return o != 0 ? bb.GetSbyte(o + bb_pos) : (sbyte)0; } }
  public ushort ID { get { int o = __offset(6); return o != 0 ? bb.GetUshort(o + bb_pos) : (ushort)0; } }
  public string Owner { get { int o = __offset(8); return o != 0 ? __string(o + bb_pos) : null; } }
  public bool IsKine { get { int o = __offset(10); return o != 0 ? 0!=bb.Get(o + bb_pos) : (bool)false; } }
  public Vec3 Pos { get { return GetPos(new Vec3()); } }
  public Vec3 GetPos(Vec3 obj) { int o = __offset(12); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public Vec4 Rot { get { return GetRot(new Vec4()); } }
  public Vec4 GetRot(Vec4 obj) { int o = __offset(14); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }

  public static void StartObject(FlatBufferBuilder builder) { builder.StartObject(6); }
  public static void AddType(FlatBufferBuilder builder, sbyte Type) { builder.AddSbyte(0, Type, 0); }
  public static void AddID(FlatBufferBuilder builder, ushort ID) { builder.AddUshort(1, ID, 0); }
  public static void AddOwner(FlatBufferBuilder builder, StringOffset OwnerOffset) { builder.AddOffset(2, OwnerOffset.Value, 0); }
  public static void AddIsKine(FlatBufferBuilder builder, bool isKine) { builder.AddBool(3, isKine, false); }
  public static void AddPos(FlatBufferBuilder builder, Offset<Vec3> posOffset) { builder.AddStruct(4, posOffset.Value, 0); }
  public static void AddRot(FlatBufferBuilder builder, Offset<Vec4> rotOffset) { builder.AddStruct(5, rotOffset.Value, 0); }
  public static Offset<Object> EndObject(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Object>(o);
  }
  public static void FinishObjectBuffer(FlatBufferBuilder builder, Offset<Object> offset) { builder.Finish(offset.Value); }
};


}
