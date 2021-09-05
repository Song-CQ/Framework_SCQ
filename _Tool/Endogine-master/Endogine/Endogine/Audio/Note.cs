namespace Endogine.Audio
{
    /// <summary>
    /// Constants representing the 12 Note of the chromatic scale.
    /// </summary>
    public enum Note
    {
        C, 
        CSharp,
        DFlat = CSharp,
        D,
        DSharp,
        EFlat = DSharp,
        E,
        F,
        FSharp,
        GFlat = FSharp,
        G,
        GSharp,
        AFlat = GSharp,
        A,
        ASharp,
        BFlat = ASharp,
        B
    }
	public class Scale
	{
		public Scale()
		{}

		//Just Pythagorean Meantone Well Equal
		public static float[] Equal
		{
			get
			{
				return new float[]{1, 1.059463f, 1.122462f, 1.189207f, 1.259921f,
									  1.334840f, 1.414214f, 1.498307f, 1.587401f, 1.681793f, 1.781797f, 1.887749f, 2};
			}
		}
		public static float[] Just
		{
			get
			{
				return new float[]{1, 135f/128, 9f/8, 6f/5, 5f/4,
									  4f/3, 45/32f, 3f/2, 8f/5, 27f/16, 9f/5, 15f/8, 2};
				//{1, 16f/15, 9f/8, 6f/5, 5f/4,
				//4f/3, 7f/5, 3f/2, 8f/5, 5f/3, 16f/9, 15f/8, 2};
			}
		}

	}
}
