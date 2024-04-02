using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // LINQ 라이브러리 추가

public class MarkerDashboard : MonoBehaviour
{
    public RectTransform content; // 스크롤 뷰의 Content 오브젝트
    public GameObject panelPrefab; // 이미지와 텍스트를 담을 패널 프리팹
    public TMP_FontAsset font; // 사용할 폰트
    public Color fontColor = Color.black; // 폰트 컬러
    public float verticalSpacing = 10f; // 이미지들 사이의 수직 간격
    private MarkerData[] markerDataArray;

    // 드랍다운 UI 요소들
    public TMP_Dropdown locationDropdown;
    public TMP_Dropdown recentDropdown;
    public TMP_Dropdown levelDropdown;

    private void Start()
    {
        // 초기 상태 설정
        locationDropdown.value = 0; // All 선택
        recentDropdown.value = 0; // Recent 선택
        levelDropdown.value = 0; // Level 선택

        // 드롭다운이 변경될 때마다 WriteDashboard 함수 호출
        locationDropdown.onValueChanged.AddListener(delegate { WriteDashboard(); });
        recentDropdown.onValueChanged.AddListener(delegate { WriteDashboard(); });
        levelDropdown.onValueChanged.AddListener(delegate { WriteDashboard(); });

        // 초기 WriteDashboard 호출
        WriteDashboard();
    }

    public void WriteDashboard(string keyword = "")
    {
        // MarkerData 배열을 필터링하여 적절한 항목만 선택
        IEnumerable<MarkerData> filteredData = FindObjectsOfType<MarkerData>();

        // Location 드롭다운에 따라 필터링
        if (locationDropdown.value != 0 && locationDropdown.options[locationDropdown.value].text != "All")
        {
            string selectedLocation = locationDropdown.options[locationDropdown.value].text;
            filteredData = filteredData.Where(data => data.location == selectedLocation);
        }

        // Level 드롭다운에 따라 필터링
        if (levelDropdown.value != 0 && levelDropdown.options[levelDropdown.value].text != "Level")
        {
            string selectedLevel = levelDropdown.options[levelDropdown.value].text;
            filteredData = filteredData.Where(data => data.level.ToString() == selectedLevel);
        }

        // 검색어가 비어있지 않다면, 키워드를 포함하는 데이터만 선택
        if (!string.IsNullOrEmpty(keyword))
        {
            filteredData = filteredData.Where(data =>
                data.id.Contains(keyword) ||
                data.information.Contains(keyword) ||
                data.location.Contains(keyword)
            );
        }

        // 최신순
        filteredData = filteredData.OrderByDescending(data => data.creationTime);

        // Recent 드롭다운에 따라 필터링
        if (recentDropdown.value != 0 && recentDropdown.options[recentDropdown.value].text == "Last")
        {
            // 과거순
            filteredData = filteredData.OrderBy(data => data.creationTime);
        }

        // 필터링된 데이터로 패널 생성
        GeneratePanels(filteredData.ToArray());
    }

    private void GeneratePanels(MarkerData[] data)
    {
        // 스크롤 뷰의 Content 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 각 마커 데이터에 대해 이미지와 텍스트를 담을 패널 생성 및 정보 표시
        foreach (MarkerData markerData in data)
        {
            // 패널 생성
            GameObject panelObject = Instantiate(panelPrefab, content);

            // 각 텍스트 컴포넌트 찾기
            TMP_Text locationText = panelObject.transform.Find("Location").GetComponent<TMP_Text>();
            TMP_Text levelText = panelObject.transform.Find("LevelNum").GetComponent<TMP_Text>();
            TMP_Text creationTimeText = panelObject.transform.Find("CreationTime").GetComponent<TMP_Text>();
            TMP_Text informationText = panelObject.transform.Find("Information").GetComponent<TMP_Text>();

            // 텍스트 설정
            locationText.font = font; // 폰트 설정
            locationText.color = fontColor;
            locationText.fontSize = 15;
            locationText.text = $"{markerData.location}";

            // 레벨에 따라 levelNum의 색상 설정
            switch (markerData.level)
            {
                case 1:
                    levelText.color = Color.red;
                    break;
                case 2:
                    levelText.color = Color.yellow;
                    break;
                case 3:
                    levelText.color = Color.green;
                    break;
                default:
                    levelText.color = fontColor;
                    break;
            }
            levelText.font = font;
            levelText.fontSize = 45;
            levelText.text = $"{markerData.level}";

            creationTimeText.font = font;
            creationTimeText.color = fontColor;
            creationTimeText.fontSize = 12;
            creationTimeText.text = $"{markerData.creationTime.ToLocalTime().ToString("yyyy-MM-dd hh:mm tt")}";

            informationText.font = font;
            informationText.color = fontColor;
            informationText.fontSize = 15;
            informationText.text = $"{markerData.information}";
        }

         // Vertical Layout Group 컴포넌트의 child force expand를 적용하여 공백을 생성
        VerticalLayoutGroup verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (verticalLayoutGroup != null)
        {
            verticalLayoutGroup.childForceExpandHeight = true;
            verticalLayoutGroup.spacing = verticalSpacing;
        }
    }
}