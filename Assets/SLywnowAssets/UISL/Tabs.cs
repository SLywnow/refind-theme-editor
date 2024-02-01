using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SLywnow.UI
{
    public class Tabs : MonoBehaviour
    {
        public List<TabsBlock> tabs;
        public int currentTab;

        public UnityEvent<int> onSwitchingTab;

        bool worked;

		private void Start()
		{
            Initialize();
        }

        int oldTab;
		private void Update()
		{
            if (oldTab != currentTab)
            {
                tabs[oldTab].tabContent.SetActive(false);
                tabs[oldTab].tabButton.interactable = true;
                oldTab = currentTab;
                tabs[currentTab].tabContent.SetActive(true);
                tabs[currentTab].tabButton.interactable = false;
                onSwitchingTab.Invoke(currentTab);
            }

        }

		public void Initialize ()
		{
            if (currentTab >= tabs.Count)
			{
                worked = false;
                Debug.LogError("currentTab is greater than or equal to the number of tabs! Script Stoped!");
                return;
            }
            worked = true;
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i].tabButton == null || tabs[i].tabContent == null)
				{
                    worked = false;
                    Debug.LogError("Tab №" + i + " not set up! Please fix this tab. Script Stoped!");
                    return;
				}
                else
				{
                    int id = i;
                    tabs[i].tabButton.onClick.AddListener(() => setTab(id));
                    if (i == currentTab && !tabs[i].disable)
                    {
                        tabs[i].tabContent.SetActive(true);
                        tabs[i].tabButton.interactable = false;
                    }
                    else if (!tabs[i].disable)
                    {
                        tabs[i].tabContent.SetActive(false);
                        tabs[i].tabButton.interactable = true;
                    }
                    else
                    {
                        tabs[i].tabContent.SetActive(false);
                        tabs[i].tabButton.interactable = false;

					}
				}
            }
		}

        public void setTab(int id)
		{
            //Debug.Log("Pressed " + id);
            if (id >= 0 && id < tabs.Count && !tabs[id].disable)
            {
                tabs[currentTab].tabContent.SetActive(false);
                tabs[currentTab].tabButton.interactable = true;
                currentTab = id;
                oldTab = id;
                tabs[currentTab].tabContent.SetActive(true);
                tabs[currentTab].tabButton.interactable = false;
                onSwitchingTab.Invoke(currentTab);
            }
        }
    }

    [System.Serializable]
    public class TabsBlock
	{
        public GameObject tabContent;
        public Button tabButton;
        public bool disable;
	}
}
