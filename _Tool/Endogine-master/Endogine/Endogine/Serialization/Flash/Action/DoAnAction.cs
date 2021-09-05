using System;
using System.Collections;

namespace Endogine.Serialization.Flash.Action
{
	/// <summary>
	/// Summary description for Action.
	/// </summary>
	public class DoAnAction : Record
	{
		public enum ActionTypes
		{
			End=0x00,
			NextFrame=0x04,
			PreviousFrame=0x05,
			Play=0x06,
			Stop=0x07,
			ToggleQuality=0x08,
			StopSound=0x09,
			Add=0x0A,
			Subtract=0x0B,
			Multiply=0x0C,
			Divide=0x0D,
			Equal=0x0E,
			LessThan=0x0F,
			LogicalAnd=0x10,
			LogicalOr=0x11,
			LogicalNot=0x12,
			StringEqual=0x13,
			StringLength=0x14,
			SubString=0x15,
			Pop=0x17,
			IntegralPart=0x18,
			GetVariable=0x1C,
			SetVariable=0x1D,
			SetTarget_Dyn=0x20,
			ConcatenateStrings=0x21,
			GetProperty=0x22,
			SetProperty=0x23,
			DuplicateSprite=0x24,
			RemoveSprite=0x25,
			Trace=0x26,
			StartDrag=0x27,
			StopDrag=0x28,
			StringLessThan=0x29,
			Throw=0x2A,
			CastObject=0x2B,
			Implements=0x2C,
			Random=0x30,
			StringLength_MB=0x31,
			Ord=0x32,
			Chr=0x33,
			GetTimer=0x34,
			SubString_MB=0x35,
			Ord_MB=0x36,
			Chr_MB=0x37,
			Delete=0x3A,
			DeleteAll=0x3B,
			SetLocalVariable=0x3C,
			CallFunction=0x3D,
			Return=0x3E,
			Modulo=0x3F,
			New=0x40,
			DeclareLocalVariable=0x41,
			DeclareArray=0x42,
			DeclareObject=0x43,
			TypeOf=0x44,
			GetTarget=0x45,
			Enumerate=0x46,
			Add_Typed=0x47,
			LessThan_Typed=0x48,
			Equal_Typed=0x49,
			Number=0x4A,
			String=0x4B,
			Duplicate=0x4C,
			Swap=0x4D,
			GetMember=0x4E,
			SetMember=0x4F,
			Increment=0x50,
			Decrement=0x51,
			CallMethod=0x52,
			NewMethod=0x53,
			InstanceOf=0x54,
			EnumerateObject=0x55,
			And=0x60,
			Or=0x61,
			XOr=0x62,
			ShiftLeft=0x63,
			ShiftRight=0x64,
			ShiftRightUnsigned=0x65,
			StrictEqual=0x66,
			GreaterThan_Typed=0x67,
			StringGreaterThan_Typed=0x68,
			Extends=0x69,
			GotoFrame=0x81,
			GetURL=0x83,
			StoreRegister=0x87,
			DeclareDictionary=0x88,
			WaitForFrame=0x8A,
			SetTarget=0x8B,
			GotoLabel=0x8C,
			WaitForFrame_Dyn=0x8D,
			DeclareFunction_V7=0x8E,
			Try=0x8F,
			With=0x94,
			PushData=0x96,
			BranchAlways=0x99,
			GetURL2=0x9A,
			DeclareFunction=0x9B,
			BranchIfTrue=0x9D,
			CallFrame=0x9E,
			GotoExpression=0x9F
		}


		public ushort Sprite;
		public ArrayList Actions;
		public DoAnAction()
		{
		}
		public override void Init(Record record)
		{
			base.Init (record);

			BinaryFlashReader reader = record.GetDataReader();
			if (this.Tag == Flash.Tags.DoInitAction)
				this.Sprite = reader.ReadUInt16();

			this.Actions = new ArrayList();
			while (true)
			{
				ActionTypes actionType = (ActionTypes)reader.ReadByte();
				//int nDataLength = 0;
				switch (actionType)
				{
					case ActionTypes.StoreRegister:
						byte register = reader.ReadByte();
						break;
					case ActionTypes.WaitForFrame_Dyn:
						byte skip = reader.ReadByte();
						break;
					case ActionTypes.GetURL2:
						byte method = reader.ReadByte();
						break;
					case ActionTypes.GotoFrame:
						ushort frameNo = reader.ReadUInt16();
						break;
					case ActionTypes.BranchAlways:
						short offset = reader.ReadInt16();
						break;
					case ActionTypes.BranchIfTrue:
						short offsetX = reader.ReadInt16();
						break;
					case ActionTypes.GotoExpression:
						//TODO: error in documentation. Says 2 bytes but declares a byte...
						byte play = reader.ReadByte();
						break;
					case ActionTypes.WaitForFrame:
						ushort frame = reader.ReadUInt16();
						byte skipX = reader.ReadByte();
						break;

					case ActionTypes.GetURL:
						string URL = reader.ReadPascalString();
						string Target = reader.ReadPascalString();
						break;
					case ActionTypes.DeclareDictionary:
						int nCount = reader.ReadUInt16();
						string[] Dictionary = new string[nCount];
						for (int i = 0; i < nCount; i++)
							reader.ReadPascalString();
						break;
					case ActionTypes.SetTarget:
						string TargetX = reader.ReadPascalString();
						break;
					case ActionTypes.GotoLabel:
						string Label = reader.ReadPascalString();
						break;
					case ActionTypes.DeclareFunction_V7:
						string Name = reader.ReadPascalString();
						ushort argCount = reader.ReadUInt16();
						byte regCount = reader.ReadByte();
						//TODO: 7 bits?
						ushort declareFunction2_reserved = reader.ReadUInt16(); //:7
						//TODO: booleans?
						ushort preloadGlobal = reader.ReadUInt16(); //:1
						ushort preloadParent = reader.ReadUInt16(); //:1
						ushort preloadRoot = reader.ReadUInt16(); //:1
						ushort suppressSuper = reader.ReadUInt16(); //:1
						ushort preloadSuper = reader.ReadUInt16(); //:1
						ushort suppressArguments = reader.ReadUInt16(); //:1
						ushort preloadArguments = reader.ReadUInt16(); //:1
						ushort suppressThis = reader.ReadUInt16(); //:1
						ushort preloadThis = reader.ReadUInt16(); //:1
						//ArrayList parameters = new ArrayList();
						for (int i = 0; i <argCount; i++)
						{
							byte paramRegister = reader.ReadByte();
							string paramName = reader.ReadPascalString();
						}
						ushort functionLength = reader.ReadUInt16();
						break;
					case ActionTypes.Try:
						//TODO: 5 bits and 1-bit bools?
						byte tryReserved = reader.ReadByte(); //: 5;
						byte catchInRegister = reader.ReadByte();//: 1;
						byte useFinally = reader.ReadByte();//: 1;
						byte useCatch = reader.ReadByte();//: 1;
						ushort trySize = reader.ReadUInt16();
						ushort catchSize = reader.ReadUInt16();
						ushort finallySize = reader.ReadUInt16();
						string catchName;
						byte catchRegister;
						if (catchInRegister == 0)
							catchName = reader.ReadPascalString();
						else
							catchRegister = reader.ReadByte();
						break;
					case ActionTypes.PushData:
						byte type = reader.ReadByte();
						//TODO: read data depending on type
						break;
					case ActionTypes.DeclareFunction:
						string NameX = reader.ReadPascalString();
						ushort argCountX = reader.ReadUInt16();
						string[] argNames = new string[argCountX];
						for (int i = 0; i < argCountX; i++)
							argNames[i] = reader.ReadPascalString();
						ushort functionLengthX = reader.ReadUInt16();
						break;
				}
			}
		}
	}
}
