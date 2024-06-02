using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CircleController : MonoBehaviour
{
    [SerializeField] public UnityEngine.UI.Toggle checkbox;
    [SerializeField] public UnityEngine.UI.Slider slider;

    public bool isMoving = false;

    private bool isOrderedMoving = false; // ���� ��� ������������ ������ �������� 

    public LineRenderer lineRenderer;

    public float speed = 5f; // �������� ����������� �������

    private List<Vector3> targetPositionsList = new List<Vector3>(); // ������ ������� �������

    private Vector3 previousTarget; 

    private int currentTargetIndex = 1; // ������ ������� ������� �������
    private int previousTargetdIndex = 0;

    private void Awake()
    {
        
          
            checkbox.onValueChanged.AddListener(delegate {
                Handle();
            }); // �������� �� ������� �������� 

    }


    private void Start()
    {
        
        lineRenderer = FindObjectOfType<LineRenderer>();
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material = whiteDiffuseMat; // ���������� ������ ��������� ��� linerenderer

        Vector3 firstPosition = transform.position;
        previousTarget = transform.position;
        firstPosition.z = 0f;
        targetPositionsList.Add(firstPosition); // ���������� ������� �������� � targetPositionsList ��� ����������� ����������


    }

   
    private void Update()
    {
        isOrderedMoving = checkbox.isOn;
        speed = slider.value;

        if (Input.GetMouseButtonDown(0)) // ���������, ���� �� ������� ����� ������ ����
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // �������� ������� ����� � ������� �����������
            targetPosition.z = 0f;
            targetPositionsList.Add(targetPosition); // ��������� ��� ������� � ������ ������� �������

            isMoving = true;
           
        }

        if (isMoving)
        {
            if (isOrderedMoving)
            {
                MoveToTargetOrdered();
            }
            else
            {
                MoveToTarget(targetPositionsList[targetPositionsList.Count - 1]); //��������� ������� ����� 

            }

            DrawLine(lineRenderer.positionCount);

        }
    }

    private void MoveToTarget(Vector3 target) //���������� ������ � ��������� ������� ������� � ���������� ���������
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        previousTarget = target;
       
        currentTargetIndex++;

        lineRenderer.positionCount++;
        DrawLine(lineRenderer.positionCount);

        if (Vector3.Distance(transform.position, previousTarget) < 0.1f) //�������� ����� ������ ��������� ���� 
        {
            isMoving = false;
        }
    }

    private void MoveToTargetOrdered() //���������� ������ � ������� �������� ��������������� �� ������ ������� �������
    {

        if (currentTargetIndex < targetPositionsList.Count)
        {
            if (Vector3.Distance(transform.position, previousTarget) < 0.1f)
            {
                lineRenderer.positionCount++;
                DrawLine(lineRenderer.positionCount);//��������� ����������

                Vector3 currentPosition = transform.position;

                previousTarget = targetPositionsList[currentTargetIndex];

                transform.position = Vector3.MoveTowards(currentPosition, targetPositionsList[currentTargetIndex], speed * Time.deltaTime);
                
                previousTargetdIndex = currentTargetIndex;
                currentTargetIndex++;
            }
            
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPositionsList[previousTargetdIndex], speed * Time.deltaTime);

        
    }

    private void DrawLine(int index) //������������ ����������
    {
        lineRenderer.SetPosition(index - 1, transform.position);
    }
    private void Handle() //������������ ������� ��������� ��������� ��������, ��������� ���������� � ������ ������ ������� �������
    {
        targetPositionsList.Clear();

        Vector3 firstPosition = transform.position;
        firstPosition.z = 0f;
        targetPositionsList.Add(firstPosition); // ���������� ������� �������� � targetPositionsList ��� ����������� ����������

        previousTarget = transform.position;
        currentTargetIndex = 1;
        previousTargetdIndex = 0;

    }
}
