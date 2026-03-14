//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.InputSystem;
//using System.Collections;

//public class ColliderPointer2D : MonoBehaviour {
//  /// <summary>
//  /// 防止鼠标悬停触发多次
//  /// </summary>
//  public bool IsHoverTriggered { get; private set; }

//  [Header("设定")]
//  [Tooltip("鼠标按下后拖拽的目标Transform")]
//  [SerializeField] private Transform draggingTarget;
//  [Tooltip("鼠标按下后，需要x秒产生拖拽效果")]
//  [SerializeField] private float draggingStartDamping = 0.5f;
//  [Tooltip("鼠标松开后，需要x秒产生丢放效果")]
//  [SerializeField] private float draggingStopDamping = 0.2f;
//  [Header("事件")]
//  [Tooltip("当鼠标点击发生事件")]
//  public UnityEvent OnClicked;
//  [Tooltip("当鼠标悬停发生事件")]
//  public UnityEvent OnHover;
//  [Tooltip("当鼠标离开发生事件")]
//  public UnityEvent OnExit;
//  [Tooltip("当鼠标拖拽发生事件")]
//  public UnityEvent OnDragged;
//  [Tooltip("当鼠标丢放发生事件")]
//  public UnityEvent OnDropped;

//  public bool enableDebugger;

//  private Collider2D _collider2D;
//  private bool isMousePressed;
//  private float draggingTimer = 0f;
//  private bool isDragging;
//  private Coroutine startDraggingTransition;
//  private Coroutine stopDraggingTransition;

//  private void Awake() {
//    _collider2D = GetComponent<Collider2D>();
//  }
//  private void Update() {
//    if (IsHoverTriggered && enabled) {
//      //On mouse not over but was hovered
//      if (_collider2D != Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()))) {
//        //Debug.Log(enabled + " : " + _collider2D + " : " + Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())));
//        //OnMouseExit();
//      }
//    }
//  }

//  public void OnMouseOver() {
//    if (IsHoverTriggered == false) OnMouseEnter();
//  }
//  public void OnMouseDown() {
//    if (enabled == false) return;

//    isMousePressed = true;

//    if (isDragging == false && draggingTarget) {
//      if (startDraggingTransition != null) StopCoroutine(startDraggingTransition);
//      startDraggingTransition = StartCoroutine(startDraggingSequence());
//    }
//  }
//  public void OnMouseUp() {
//    if (enabled == false) return;

//    isMousePressed = false;

//    if (isDragging == true && draggingTarget) {
//      if (stopDraggingTransition != null) StopCoroutine(stopDraggingTransition);
//      stopDraggingTransition = StartCoroutine(stopDraggingSequence());
//    }

//    onMouseClicked();

//    //ScreenToWorldPoint is not working in windows build
//    //mouseUpPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

//    //if (_collider2D == Physics2D.OverlapPoint(mouseUpPosition) &&
//    //  Vector3.Distance(mouseDownPosition, mouseUpPosition) <= 0.1) {
//    //  onMouseClicked();
//    //}

//    //onMouseClicked();
//  }
//  public void OnMouseEnter() {
//    if (enabled == false) return;
//    if (enableDebugger) Debug.Log(name + " 鼠标已经悬停 " + IsHoverTriggered);
//    if (IsHoverTriggered) return; // 防止重复触发

//    IsHoverTriggered = true;
   
//    OnHover.Invoke();
//  }
//  public void OnMouseExit() {
//    if (enabled == false) return;

//    if (enableDebugger) Debug.Log(name + " 鼠标已经退出");
//    IsHoverTriggered = false;
//    OnExit.Invoke();
//  }

//  private void onMouseClicked() {
//    if (enableDebugger) Debug.Log(name + " 鼠标已经点击");

    
//    OnClicked.Invoke();
//  }

//  private IEnumerator startDraggingSequence() {
//    draggingTimer = 0f;
//    while (draggingTimer < draggingStartDamping) {
//      draggingTimer += Time.deltaTime;
//      yield return null;
//    }

//    isDragging = true;
//    OnDragged.Invoke();

//    while (isDragging) {
//      draggingTarget.position = (Vector2)Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
//      yield return null;
//    }
//  }
//  private IEnumerator stopDraggingSequence() {
//    draggingTimer = 0f;
//    while (draggingTimer < draggingStopDamping) {
//      if (isMousePressed) yield break; // 如果鼠标仍然按下，则不执行丢放效果
//      draggingTimer += Time.deltaTime;
//      yield return null;
//    }

//    isDragging = false;
//    OnDropped.Invoke();
//  }
//}
