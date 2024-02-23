using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SearchController : MonoBehaviour
{
    public MarkerDashboard markerPanel;

    private TMP_InputField searchInputField;

    private void Start()
    {
        // TMP_InputField 컴포넌트 가져오기
        searchInputField = GetComponent<TMP_InputField>();

        // 엔터 키로도 검색 가능하게 함
        searchInputField.onSubmit.AddListener(delegate { Search(); });
    }

    public void Search()
    {
        // 검색어 가져오기
        string keyword = searchInputField.text;
        
        // MarkerPanel의 WriteDashboard 메서드 호출하여 검색어 전달
        markerPanel.WriteDashboard(keyword);

        // 텍스트 입력란 초기화
        searchInputField.text = "";
    }
}
