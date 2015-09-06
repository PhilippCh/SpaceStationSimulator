using System;

namespace SpaceStation.Station.Object {

	[AttributeUsage(AttributeTargets.Class)]
	public class RegisterAsObject : Attribute {

		public RegisterAsObject(short id, Type objectType)
		{
			this.id = id;
			this.objectType = objectType;
		}

		protected short id;
		public short Id {
			get {
				return this.id;
			}
		}

		protected Type objectType;
		public Type ObjectType {
			get {
				return this.objectType;
			}
		}
	}

}