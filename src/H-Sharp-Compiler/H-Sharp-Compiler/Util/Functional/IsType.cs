namespace HSharp.Util.Functional {

    /// <summary>
    /// Functionally tied data structure for checking if some object T is of type TElse.
    /// </summary>
    /// <typeparam name="T">Source object type</typeparam>
    public struct IsType<T> {

        private T m_testee;
        private bool m_any;

        /// <summary>
        /// Initialize IsType chain with root object
        /// </summary>
        /// <param name="o">The root object to check type with</param>
        public IsType(T o) {
            this.m_testee = o;
            this.m_any = false;
        }

        private IsType<T> Yes() { this.m_any = true; return this; }

        /// <summary>
        /// Check if object is of type TElse.
        /// </summary>
        /// <typeparam name="TElse">The type to check.</typeparam>
        /// <returns>Self with internal flag flipped if true.</returns>
        public IsType<T> Is<TElse>() => this.Or<TElse>() ? this.Yes() : this;

        /// <summary>
        /// Check if the object is otherwise of type TElse.
        /// </summary>
        /// <typeparam name="TElse">The type to check.</typeparam>
        /// <returns>True if any type matched throughout the chain or is of type TElse. Otherwise false.</returns>
        public bool Or<TElse>() => this.m_testee is TElse || this.m_any;

    }

}
