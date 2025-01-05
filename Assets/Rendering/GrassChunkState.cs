
public enum GrassChunkState {
        None,

        /// <summary>
        /// 読み込み中
        /// </summary>
        Loading,

        /// <summary>
        /// 読み込み済み
        /// </summary>
        Loaded,

        /// <summary>
        /// 破棄中
        /// </summary>
        Disposing,

        /// <summary>
        /// 破棄済み
        /// </summary>
        Disposed,
    }
