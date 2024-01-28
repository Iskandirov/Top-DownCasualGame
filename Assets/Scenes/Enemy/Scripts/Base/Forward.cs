using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
[DefaultExecutionOrder(6)]
public class Forward : MonoBehaviour
{
    //public GameObject enemy;
    //public Rigidbody2D Body;
    //public float speedMax;
    //public float currentSpeed;
    ////public bool isStunned;
    //public float stunnTime;
    //public float stunnTimeMax;
    //public bool isFly;

    //public bool isThree;
    //public Animator anim;
    //public bool IsOutOfCamera;

    //public bool isChaising;

    //public float acceleration = 0.1f; // збільшення швидкості
    //public float angle;

    //private Vector2 velocity;
    //public AIPath path;
    //public bool isTutorial;
    //public float moveSlowTime;
    //public PlayerManager player;
    //public Transform objTransform;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    if (!isTutorial)
    //    {
    //        if (GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) > 0)
    //            speedMax += GameManager.Instance.LoadObjectLevelCount(SceneManager.GetActiveScene().buildIndex) * 1.1f;
    //    }
    //    player = PlayerManager.instance;
    //    objTransform = transform;
    //    //walk.Play();
    //    if (isChaising)
    //    {
    //        // встановлюємо вектор швидкості в початкове значення (нульовий вектор)
    //        velocity = Vector2.zero;
    //        gameObject.AddComponent<Rigidbody2D>();
    //        Body = GetComponent<Rigidbody2D>();
    //        Body.gravityScale = 0;
    //        Body.angularDrag = 0;
    //        Body.mass = 5000;
    //        Body.freezeRotation = true;
    //    }
    //    else
    //    {
    //        // Код для режиму редактора Unity
    //        //isStunned = false;
    //    }
    //    //destination.target = player.objTransform;
    //    currentSpeed = speedMax;
    //    //path.maxSpeed = currentSpeed;
    //}
    //bool IsObjectOutsideCameraBounds(Vector2 objPosition)
    //{
    //    Camera mainCamera = Camera.main;

    //    Vector3 objectViewportPosition = mainCamera.WorldToViewportPoint(objPosition);

    //    bool isOutsideBounds = objectViewportPosition.x < -0.5f || objectViewportPosition.x > 1.45f ||
    //                           objectViewportPosition.y < -0.5f || objectViewportPosition.y > 1.45f;

    //    IsOutOfCamera = isOutsideBounds;
    //    return isOutsideBounds;
    //}
    //public void StopMove()
    //{
    //    path.maxSpeed = 0;
    //}

    //public void StartMove()
    //{
    //    if (path != null)
    //    {
    //        path.maxSpeed = currentSpeed;
    //    }
    //}
    //public IEnumerator SlowEnemy(float time, float percent)
    //{
    //    currentSpeed = speedMax * percent;
    //    yield return new WaitForSeconds(time);
    //    currentSpeed = speedMax;
    //}
    //private void FixedUpdate()
    //{
    //    if (!isChaising)
    //    {
    //        if (IsObjectOutsideCameraBounds(objTransform.position))
    //        {
    //            anim.enabled = false;
    //        }
    //        else
    //        {
    //            anim.enabled = true;
    //        }
    //        //if (isStunned)
    //        {
    //            Stuned();
    //        }

    //        if (Body.position.x < player.transform.position.x)
    //        {
    //            Body.transform.rotation = Quaternion.identity;
    //        }
    //        else
    //        {
    //            Body.transform.rotation = Quaternion.Euler(0, 180, 0);
    //        }
    //    }
    //    else
    //    {
    //        // визначаємо напрямок до гравця
    //        Vector2 direction = player.transform.position - objTransform.position;
    //        direction.Normalize();

    //        // визначаємо кут між напрямком до гравця і поточним напрямком руху об'єкту
    //        angle = Vector2.Angle(velocity, direction);

    //        // якщо кут між векторами більший за 20 градусів - зменшуємо швидкість
    //        if (angle > 20f)
    //        {
    //            // зменшуємо швидкість з поступовим нарощуванням
    //            velocity = Vector2.Lerp(velocity, direction * speedMax, Time.fixedDeltaTime * acceleration);
    //        }
    //        else // якщо кут менший - збільшуємо швидкість
    //        {
    //            // збільшуємо швидкість з поступовим нарощуванням
    //            velocity = Vector2.Lerp(velocity, direction * speedMax, Time.fixedDeltaTime * acceleration);
    //        }

    //        // обмежуємо максимальну швидкість
    //        velocity = Vector2.ClampMagnitude(velocity, speedMax);

    //        // зміщуємо об'єкт на відстань, що дорівнює швидкості, помноженій на час оновлення
    //        objTransform.Translate(velocity * Time.fixedDeltaTime);
    //    }
    //}

    //public void Stuned()
    //{
    //    if (stunnTime <= 0)
    //    {
    //        // Код для режиму редактора Unity
    //        currentSpeed = speedMax;
    //        //isStunned = false;
    //        stunnTime = stunnTimeMax;
    //    }
    //    else
    //    {
    //        currentSpeed = 0;
    //        stunnTime -= Time.fixedDeltaTime;
    //    }
    //}
}