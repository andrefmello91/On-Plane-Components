namespace andrefmello91.OnPlaneComponents
{
	/// <summary>
	///     Cases of <see cref="PrincipalStrainState" /> and <see cref="PrincipalStressState" />.
	/// </summary>
	public enum PrincipalCase
	{
		/// <summary>
		///     Tension at both directions.
		/// </summary>
		PureTension,
		
		/// <summary>
		///     Compression at both directions.
		/// </summary>
		PureCompression,
		
		/// <summary>
		///     Tension at one direction and compression at another one..
		/// </summary>
		TensionCompression,
		
		/// <summary>
		///		Tension at one direction, no stress at another.
		/// </summary>
		UniaxialTension,
		
		/// <summary>
		///		Compression at one direction, no stress at another.
		/// </summary>
		UniaxialCompression,

		/// <summary>
		///     The state has no stress/strain.
		/// </summary>
		Zero
	}
}