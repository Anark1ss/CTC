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

    private bool isOrderedMoving = false; // флаг для переключения режима движения 

    public LineRenderer lineRenderer;

    public float speed = 5f; // Скорость перемещения объекта

    private List<Vector3> targetPositionsList = new List<Vector3>(); // Список целевых позиций

    private Vector3 previousTarget; 

    private int currentTargetIndex = 1; // Индекс текущей целевой позиции
    private int previousTargetdIndex = 0;

    private void Awake()
    {
        
          
            checkbox.onValueChanged.AddListener(delegate {
                Handle();
            }); // Подписка на нажатие чекбокса 

    }


    private void Start()
    {
        
        lineRenderer = FindObjectOfType<LineRenderer>();
        Material whiteDiffuseMat = new Material(Shader.Find("Unlit/Texture"));
        lineRenderer.material = whiteDiffuseMat; // Добавленик белого материала для linerenderer

        Vector3 firstPosition = transform.position;
        previousTarget = transform.position;
        firstPosition.z = 0f;
        targetPositionsList.Add(firstPosition); // Добавление первого элемента в targetPositionsList для отображение траектории


    }

   
    private void Update()
    {
        isOrderedMoving = checkbox.isOn;
        speed = slider.value;

        if (Input.GetMouseButtonDown(0)) // Проверяем, было ли нажатие левой кнопки мыши
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Получаем позицию клика в мировых координатах
            targetPosition.z = 0f;
            targetPositionsList.Add(targetPosition); // Добавляем эту позицию в список целевых позиций

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
                MoveToTarget(targetPositionsList[targetPositionsList.Count - 1]); //последнее нажатое место 

            }

            DrawLine(lineRenderer.positionCount);

        }
    }

    private void MoveToTarget(Vector3 target) //Перемещает объект к указанной целевой позиции с постоянной скоростью
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        previousTarget = target;
       
        currentTargetIndex++;

        lineRenderer.positionCount++;
        DrawLine(lineRenderer.positionCount);

        if (Vector3.Distance(transform.position, previousTarget) < 0.1f) //проверка когда обьект достигнет цели 
        {
            isMoving = false;
        }
    }

    private void MoveToTargetOrdered() //Перемещает объект к целевым позициям последовательно из списка целевых позиций
    {

        if (currentTargetIndex < targetPositionsList.Count)
        {
            if (Vector3.Distance(transform.position, previousTarget) < 0.1f)
            {
                lineRenderer.positionCount++;
                DrawLine(lineRenderer.positionCount);//отрисовка траектории

                Vector3 currentPosition = transform.position;

                previousTarget = targetPositionsList[currentTargetIndex];

                transform.position = Vector3.MoveTowards(currentPosition, targetPositionsList[currentTargetIndex], speed * Time.deltaTime);
                
                previousTargetdIndex = currentTargetIndex;
                currentTargetIndex++;
            }
            
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPositionsList[previousTargetdIndex], speed * Time.deltaTime);

        
    }

    private void DrawLine(int index) //Отрисовывает траекторию
    {
        lineRenderer.SetPosition(index - 1, transform.position);
    }
    private void Handle() //Обрабатывает событие изменения состояния чекбокса, сбрасывая переменные и очищая список целевых позиций
    {
        targetPositionsList.Clear();

        Vector3 firstPosition = transform.position;
        firstPosition.z = 0f;
        targetPositionsList.Add(firstPosition); // добавление первого элемента в targetPositionsList для отображение траектории

        previousTarget = transform.position;
        currentTargetIndex = 1;
        previousTargetdIndex = 0;

    }
}
