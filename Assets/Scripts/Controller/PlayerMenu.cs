using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Zombie
{
    public class PlayerMenu : MonoBehaviour
    {
        public GameObject playerPanel;
        public GameObject upperMenuButtons;
        KeyCode currentInput;
        public bool isopenInventory = false;

        public GameObject statPanel;
        public GameObject inventoryPanel;
        public GameObject mapPanel;
        public GameObject companionPanel;
        public GameObject tasksPanel;

        private void Start()
        {
            playerPanel.SetActive(false);
            upperMenuButtons.SetActive(false);
            isopenInventory = false;
            statPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            mapPanel.SetActive(false);
            companionPanel.SetActive(false);
            tasksPanel.SetActive(false);
        }

        private void Update()
        {
            if (isopenInventory)
            {
                Time.timeScale = 0;
                if (Input.GetKeyDown(KeyCode.C))
                {
                    if (currentInput == KeyCode.C)
                    {
                        SetDeactive();
                    }
                    else
                    {
                        SetActivePlayerMenu(0);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    if (currentInput == KeyCode.I)
                    {
                        SetDeactive();
                    }
                    else
                    {
                        SetActivePlayerMenu(1);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    if (currentInput == KeyCode.M)
                    {
                        SetDeactive();
                    }
                    else
                    {
                        SetActivePlayerMenu(2);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    if (currentInput == KeyCode.P)
                    {
                        SetDeactive();
                    }
                    else
                    {
                        SetActivePlayerMenu(3);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    if (currentInput == KeyCode.T)
                    {
                        SetDeactive();
                    }
                    else
                    {
                        SetActivePlayerMenu(4);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetDeactive();
                }

            }
            else
            {
                Time.timeScale = 1;
                if (Input.GetKeyDown(KeyCode.C))
                {
                    SetActivePlayerMenu(0);
                    isopenInventory = true;
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    SetActivePlayerMenu(1);
                    isopenInventory = true;
                }
                else if (Input.GetKeyDown(KeyCode.M))
                {
                    SetActivePlayerMenu(2);
                    isopenInventory = true;

                }
                else if (Input.GetKeyDown(KeyCode.P))
                {
                    SetActivePlayerMenu(3);
                    isopenInventory = true;

                }
                else if (Input.GetKeyDown(KeyCode.T))
                {
                    SetActivePlayerMenu(4);
                    isopenInventory = true;
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    //SetDeactive();
                }
            }
        }

        public void SetActivePlayerMenu(int num)
        {
            playerPanel.SetActive(true);
            upperMenuButtons.SetActive(true);
            statPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            mapPanel.SetActive(false);
            companionPanel.SetActive(false);
            tasksPanel.SetActive(false);

            if (num == 0)
            {
                statPanel.SetActive(true);
                SetKeyCode(KeyCode.C);
            }
            else if (num == 1)
            {
                inventoryPanel.SetActive(true);
                SetKeyCode(KeyCode.I);
            }
            else if (num == 2)
            {
                mapPanel.SetActive(true);
                SetKeyCode(KeyCode.M);
            }
            else if (num == 3)
            {
                companionPanel.SetActive(true);
                SetKeyCode(KeyCode.P);
            }
            else if (num == 4)
            {
                tasksPanel.SetActive(true);
                SetKeyCode(KeyCode.T);
            }
        }

        public void SetDeactive()
        {
            statPanel.SetActive(false);
            inventoryPanel.SetActive(false);
            mapPanel.SetActive(false);
            companionPanel.SetActive(false);
            tasksPanel.SetActive(false);
            upperMenuButtons.SetActive(false);
            playerPanel.SetActive(false);
            isopenInventory = false;
        }

        public void SetKeyCode(KeyCode keyCode)
        {
            currentInput = keyCode;
        }
    }
}