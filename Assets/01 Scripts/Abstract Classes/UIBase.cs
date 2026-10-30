using UnityEngine;

// Open, Close 함수는 필요한 경우 오버라이드해서 사용해도 됨.
// 단 부모 가상함수 호출 잊지 않기 ex) base.Close()
public class UIBase : MonoBehaviour {
    public virtual void Open() {
        gameObject.SetActive(true);
    }

    public virtual void Close() {
        gameObject.SetActive(false);
    }
}
