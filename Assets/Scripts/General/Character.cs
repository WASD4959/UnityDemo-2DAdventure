using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    private PlayerController playerController;

    [Header("事件监听")]
    public VoidEventSO newGameEvent;
    public VoidEventSO CameraShakeEvent;

    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    
    [Header("受伤无敌")]
    public float invulnerableDuration;
    [HideInInspector] public float invulnerableCounter;
    public bool invulnerable;
    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;

    private void Awake() {
        playerController = GetComponent<PlayerController>();
    }

    private void Start() {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void NewGame() {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void Update() {
        if(invulnerable) {
            invulnerableCounter -= Time.deltaTime;
            if(invulnerableCounter <= 0) {
                invulnerable = false;
            }
        }

        if(currentPower < maxPower){
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }
    private void OnEnable() {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        // saveable.RegisterSaveData();

        if (saveable != null) {
            saveable.RegisterSaveData();
        } else {
            Debug.LogError("Saveable interface is not implemented correctly.");
        }
    }

    private void OnDisable() {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(currentHealth != 0 && other.CompareTag("water")){
            currentHealth = 0.0f;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public void TakeDamage(Attack attacker) {
        if(invulnerable)
            return;

        if(playerController != null && playerController.isBlock){
            if(currentPower - attacker.damage > 0){
                currentPower -= attacker.damage;
                TriggerInvulnerable();
                CameraShakeEvent.RaiseEvent();
                OnHealthChange?.Invoke(this);
                return;
            }
            else
            {
                currentPower = 0;
                playerController.isBlock = false;
            }
        }
            
        if(currentHealth - attacker.damage > 0) {
            currentHealth -= attacker.damage;
            if(attacker.damage != 0){
                TriggerInvulnerable();
                //执行受伤
                OnTakeDamage?.Invoke(attacker.transform);
            }
        }
        else {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }

        OnHealthChange?.Invoke(this);
    }
    
    private void TriggerInvulnerable() {
        if(!invulnerable) {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost){
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }

    public DataDefinition GetDataID()
    {
        return GetComponent<DataDefinition>();
    }

    public void GetSaveData(Data data)
    {
        if(data.characterPosDic.ContainsKey(GetDataID().ID)){
            data.characterPosDic[GetDataID().ID] = new SerializeVector3(transform.position);
            data.floatSavedData[GetDataID().ID + "_health"] = this.currentHealth;
            data.floatSavedData[GetDataID().ID + "_power"] = this.currentPower;
        }else{
            data.characterPosDic.Add(GetDataID().ID, new SerializeVector3(transform.position));
            data.floatSavedData.Add(GetDataID().ID + "_health", this.currentHealth);
            data.floatSavedData.Add(GetDataID().ID + "_power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        if(data.characterPosDic.ContainsKey(GetDataID().ID)){
            transform.position = data.characterPosDic[GetDataID().ID].DeserializeVector3();
            this.currentHealth = data.floatSavedData[GetDataID().ID + "_health"];
            this.currentPower = data.floatSavedData[GetDataID().ID + "_power"];

            //更新UI
            OnHealthChange?.Invoke(this);
        }
    }
}
