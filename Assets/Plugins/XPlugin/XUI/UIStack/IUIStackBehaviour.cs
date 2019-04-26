namespace XUI {
    public interface IUIStackBehaviour {
        void OnUIShow(params object[] args);
        void OnUIClose();

        void OnUIPause(bool cover);
        void OnUIResume(bool fromCover);
    }
}