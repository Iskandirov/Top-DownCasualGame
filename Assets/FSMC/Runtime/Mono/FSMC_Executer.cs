using UnityEngine;
using UnityEngine.UI;

namespace FSMC.Runtime
{
    public class FSMC_Executer : MonoBehaviour
    {
        private FSMC_Controller _runtimeController;
        [SerializeField] private FSMC_Controller _controller;
        public Animator anim;
        public static FSMC_Executer instance;
        //public string mobName;
        [Header("Move")]
        public PolygonCollider2D mapBounds;
        public float speed;
        public float speedMax;
        [Header("Attack")]
        float damage;
        public float attackSpeed;
        public float attackSpeedMax;
        public Transform objTransform;
        [SerializeField] public GameObject bullet;
        [Header("Health")]
        public float health;
        public float healthMax;
        public bool isSpawnOutsideCamera;
        public GameObject expiriancePoint;
        [Header("Stats")]
        public float expGiven;
        public float dangerLevel;
        public int enemyType;
        [Header("Boss")]
        public GameObject bossHealthObj;
        public Image healthObjImg;
        public bool isBoss;
        public FSMC_Controller StateMachine { get { return _runtimeController; } set { if (value != null && value != _runtimeController) { _runtimeController = value; _runtimeController.StartStateMachine(this); } } }

        public void DealDamage()
        {
            Instantiate(bullet, transform.position, Quaternion.identity);
        }
        public void MuteSpeed()
        {
            speed = 0;
        } 
        public void UnMuteSpeed()
        {
            speed = speedMax;
        }
        public float GetDamage()
        {
            return damage;
        } 
        void SetDamage(float Damage)
        {
            damage = Damage;
        }
        public void TakeDamage(float damage)
        {
            SetDamage(damage);
            if (health > 0)
            {
                StateMachine.SetCurrentState("Hit", this);
            }
            health -= damage;
        }
        private void Awake()
        {
            instance = this;
            objTransform = transform;
            speed = speedMax;
            anim = GetComponent<Animator>();
            _runtimeController = Instantiate(_controller);
            StateMachine.SetInt("EnemyType", enemyType);
        }


        void Start()
        {
            _runtimeController.StartStateMachine(this);
        }

        void Update()
        {
            _runtimeController.UpdateStateMachine(this);
        }
        public void ReturnToChase()
        {
            anim.SetBool("Death",false);
        }

        public void SetFloat(string name, float value)
        {
            _runtimeController.SetFloat(name, value);
        }
        public void SetFloat(int id, float value)
        {
            _runtimeController.SetFloat(id, value);
        }

        public float GetFloat(string name)
        {
            return _runtimeController.GetFloat(name);
        }
        public float GetFloat(int id)
        {
            return _runtimeController.GetFloat(id);
        }

        public void SetInt(string name, int value)
        {
            _runtimeController.SetInt(name, value);
        }
        public void SetInt(int id, int value)
        {
            _runtimeController.SetInt(id, value);
        }
        public int GetInt(string name)
        {
            return _runtimeController.GetInt(name);
        }
        public int GetInt(int id)
        {
            return _runtimeController.GetInt(id);
        }
        public void SetBool(string name, bool value)
        {
            _runtimeController.SetBool(name, value);
        }
        public void SetBool(int id, bool value)
        {
            _runtimeController.SetBool(id, value);
        }
        public bool GetBool(string name)
        {
            return _runtimeController.GetBool(name);
        }
        public bool GetBool(int id)
        {
            return _runtimeController.GetBool(id);
        }
        public void SetTrigger(string name)
        {
            _runtimeController.SetTrigger(name);
        }
        public void SetTrigger(int id)
        {
            _runtimeController.SetTrigger(id);
        }

        public FSMC_State GetCurrentState()
        {
            return _runtimeController.GetCurrentState();
        }
        public void SetCurrentState(string name)
        {
            _runtimeController.SetCurrentState(name, this);
        }
        public FSMC_State GetState(string name)
        {
            return _runtimeController.GetState(name);
        }
    }
}
