using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float lives = 3;
    public bool isAlive = true;
    public Vector3 respawnPosition;
    public float respawnTime = 1;
    public float respawnInvulnerabilityTime = 1.5f;
    public bool canTakeDamage = true;
    public float typeChangeCooldown = 0.10f;
    [SerializeField] public AudioSource soundSource;

    [SerializeField] public GameObject deathMenu; //pelo amor de deus remova essa gambiarra depois e fa�a um evento de morte direito no gamemanager

    [SerializeField] public MeshRenderer currentMeshRenderer;
    [SerializeField] public SkinnedMeshRenderer currentSkinnedMeshRenderer; 
    [SerializeField] public MeshRenderer rockMeshRenderer;
    [SerializeField] public MeshRenderer paperMeshRenderer;
    [SerializeField] public SkinnedMeshRenderer scissorsMeshRenderer;
    Color paperOriginalColor, rockOriginalColor, scissorsOriginalColor;

    [SerializeField] public PlayerType currentType = PlayerType.Paper;
    public bool canSwitchType = true;
    public bool canBecomeRock = false;
    public bool canBecomeScissors = false;
    [SerializeField] GameObject playerRockObject;
    [SerializeField] GameObject playerPaperObject;
    [SerializeField] GameObject playerScissorsObject;
    [SerializeField] BoxCollider rockCollider;
    [SerializeField] BoxCollider paperCollider;
    [SerializeField] BoxCollider scissorsCollider;
    [SerializeField] GameObject typeChangeVFX;
    [SerializeField] GameObject rockDeathVFX;
    [SerializeField] GameObject paperDeathVFX;
    [SerializeField] GameObject scissorsDeathVFX;
    [SerializeField] GameObject rockShieldVFX;
    [SerializeField] GameObject paperShieldVFX;
    [SerializeField] GameObject scissorsShieldVFX;
    public GameObject morphShape;
    public Material morphMaterial;
    public Animator morphAnimator;
    public Transform scissorVisual;
    float morphTime = 0;
    float morphRate = 0;
    bool isMorphing = false;
    public bool isRespawning = false;
    public bool hasTypeUI = false;
    public TypeHUD typeHUD;

    private void Start()
    {
        morphMaterial = morphShape.GetComponentInChildren<Renderer>().material;
        if (currentType == PlayerType.Paper) currentMeshRenderer = paperMeshRenderer;
        if (currentType == PlayerType.Scissors) {
            currentSkinnedMeshRenderer = scissorsMeshRenderer;
            currentMeshRenderer = paperMeshRenderer;
        } 
        if (currentType == PlayerType.Rock) currentMeshRenderer = rockMeshRenderer;
        paperOriginalColor = paperMeshRenderer.material.color;
        rockOriginalColor = rockMeshRenderer.material.color;
        scissorsOriginalColor = scissorsMeshRenderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentType == PlayerType.Rock && other.CompareTag("BulletEnemyRock") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Rock && other.CompareTag("BulletEnemyPaper") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Rock && other.CompareTag("BulletEnemyScissors") && canTakeDamage) IgnoreDamage();

        if (currentType == PlayerType.Paper && other.CompareTag("BulletEnemyRock") && canTakeDamage) IgnoreDamage();
        if (currentType == PlayerType.Paper && other.CompareTag("BulletEnemyPaper") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Paper && other.CompareTag("BulletEnemyScissors") && canTakeDamage) StartCoroutine(DieOrRespawn());

        if (currentType == PlayerType.Scissors && other.CompareTag("BulletEnemyRock") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Scissors && other.CompareTag("BulletEnemyPaper") && canTakeDamage) IgnoreDamage();
        if (currentType == PlayerType.Scissors && other.CompareTag("BulletEnemyScissors") && canTakeDamage) StartCoroutine(DieOrRespawn());



        if (currentType == PlayerType.Rock && other.CompareTag("EnemyRock") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Rock && other.CompareTag("EnemyPaper") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Rock && other.CompareTag("EnemyScissors") && canTakeDamage) IgnoreDamage();

        if (currentType == PlayerType.Paper && other.CompareTag("EnemyRock") && canTakeDamage) IgnoreDamage();
        if (currentType == PlayerType.Paper && other.CompareTag("EnemyPaper") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Paper && other.CompareTag("EnemyScissors") && canTakeDamage) StartCoroutine(DieOrRespawn());

        if (currentType == PlayerType.Scissors && other.CompareTag("EnemyRock") && canTakeDamage) StartCoroutine(DieOrRespawn());
        if (currentType == PlayerType.Scissors && other.CompareTag("EnemyScissors") && canTakeDamage) StartCoroutine(DieOrRespawn());

        if (other.CompareTag("Extra Life")) GetALife();
        if (other.CompareTag("RockPowerUp"))
        {
            canBecomeRock = true;
            StartCoroutine(SwitchTypeToRock());
        }
        if (other.CompareTag("ScissorsPowerUp"))
        {
            canBecomeScissors = true;
            StartCoroutine(SwitchTypeToScissors());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale += 0.10f;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Time.timeScale -= 0.10f;
        }

        if (Input.GetButtonDown("NextType") && currentType == PlayerType.Rock && canSwitchType) StartCoroutine(SwitchTypeToPaper());
        if (Input.GetButtonDown("PreviousType") && currentType == PlayerType.Rock && canSwitchType && canBecomeScissors) StartCoroutine(SwitchTypeToScissors());
        else if (Input.GetButtonDown("PreviousType") && currentType == PlayerType.Rock && canSwitchType && !canBecomeScissors) StartCoroutine(SwitchTypeToPaper());

        if (Input.GetButtonDown("NextType") && currentType == PlayerType.Paper && canSwitchType && canBecomeScissors) StartCoroutine(SwitchTypeToScissors());
        else if (Input.GetButtonDown("NextType") && currentType == PlayerType.Paper && canSwitchType && !canBecomeScissors && canBecomeRock) StartCoroutine(SwitchTypeToRock());
        if (Input.GetButtonDown("PreviousType") && currentType == PlayerType.Paper && canSwitchType && canBecomeRock) StartCoroutine(SwitchTypeToRock());
        else if (Input.GetButtonDown("PreviousType") && currentType == PlayerType.Paper && canSwitchType && !canBecomeRock && canBecomeScissors) StartCoroutine(SwitchTypeToScissors());

        if (Input.GetButtonDown("NextType") && currentType == PlayerType.Scissors && canSwitchType && canBecomeRock) StartCoroutine(SwitchTypeToRock());
        else if (Input.GetButtonDown("NextType") && currentType == PlayerType.Scissors && canSwitchType && !canBecomeRock) StartCoroutine(SwitchTypeToPaper());
        if (Input.GetButtonDown("PreviousType") && currentType == PlayerType.Scissors && canSwitchType) StartCoroutine(SwitchTypeToPaper());

        if (Input.GetKeyDown(KeyCode.F3)) lives = 999;
        if (Input.GetKeyDown(KeyCode.F5)) lives = 2;
        if (Input.GetKeyDown(KeyCode.F4)){ canBecomeRock = true; canBecomeScissors = true; }

        if (isMorphing)
        {
            morphTime += Time.deltaTime;
            morphRate = morphTime * 6.6666666f;
            morphMaterial.SetFloat("_changeRate", morphRate);
        }
    }
    IEnumerator SwitchTypeToRock()
    {
        //GameObject changeVfx = Instantiate(typeChangeVFX, transform.position, transform.rotation);
        //changeVfx.transform.parent = this.transform;
        currentMeshRenderer.enabled = false;
        currentSkinnedMeshRenderer.enabled = false;
        canSwitchType = false;

        morphShape.SetActive(true);
        if(currentType == PlayerType.Paper)
        {
            morphAnimator.SetTrigger("PaperToRock");
            StartCoroutine(CountMorphTime(0));
            if(hasTypeUI) typeHUD.ChangeTypeUI(2);
        } 
        if (currentType == PlayerType.Scissors)
        {
            morphAnimator.SetTrigger("ScissorsToRock");
            StartCoroutine(CountMorphTime(4));
            if(hasTypeUI) typeHUD.ChangeTypeUI(4);
        } 

        yield return new WaitForSeconds(0.15f);

        morphShape.SetActive(false);
        playerRockObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        if(isRespawning){
         currentMeshRenderer.enabled = false;
         rockMeshRenderer.enabled = false;
        }
        else{
            rockMeshRenderer.enabled = true;
            currentMeshRenderer.enabled = true;
        }
        currentType = PlayerType.Rock;
        playerRockObject.SetActive(true);
        playerPaperObject.SetActive(false);
        playerScissorsObject.SetActive(false);
        rockCollider.enabled = true;
        paperCollider.enabled = false;
        scissorsCollider.enabled = false;
        currentMeshRenderer = rockMeshRenderer;
        if (rockMeshRenderer.material.color != rockOriginalColor) rockMeshRenderer.material.color = rockOriginalColor;
        StartCoroutine(CountTransformCooldown());
        //Debug.Log("You are now a Rock!");
    }

    IEnumerator SwitchTypeToPaper()
    {
        //GameObject changeVfx = Instantiate(typeChangeVFX, transform.position, transform.rotation);
        //changeVfx.transform.parent = this.transform;
        currentMeshRenderer.enabled = false;
        currentSkinnedMeshRenderer.enabled = false;
        canSwitchType = false;

        morphShape.SetActive(true);
        if(currentType == PlayerType.Rock)
        {
            morphAnimator.SetTrigger("RockToPaper");
            StartCoroutine(CountMorphTime(2));
            if(hasTypeUI) typeHUD.ChangeTypeUI(0);
        } 
        if (currentType == PlayerType.Scissors)
        {
            morphAnimator.SetTrigger("ScissorsToPaper");
            StartCoroutine(CountMorphTime(5));
            if(hasTypeUI) typeHUD.ChangeTypeUI(5);
        }

        yield return new WaitForSeconds(0.15f);

        morphShape.SetActive(false);
        playerPaperObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        if(isRespawning){
         currentMeshRenderer.enabled = false;
         paperMeshRenderer.enabled = false;
        }
        else{
            currentMeshRenderer.enabled = true;
            paperMeshRenderer.enabled = true;
        }
        currentType = PlayerType.Paper;
        playerPaperObject.SetActive(true);
        playerRockObject.SetActive(false);
        playerScissorsObject.SetActive(false);
        rockCollider.enabled = false;
        paperCollider.enabled = true;
        scissorsCollider.enabled = false;
        currentMeshRenderer = paperMeshRenderer;
        if (paperMeshRenderer.material.color != paperOriginalColor) paperMeshRenderer.material.color = paperOriginalColor;
        StartCoroutine(CountTransformCooldown());
        //Debug.Log("You are now a Paper!");
    }
    

    IEnumerator SwitchTypeToScissors()
    {
        //GameObject changeVfx = Instantiate(typeChangeVFX, transform.position, transform.rotation);
        //changeVfx.transform.parent = this.transform;
        currentMeshRenderer.enabled = false;
        canSwitchType = false;

        morphShape.SetActive(true);
        if(currentType == PlayerType.Rock)
        {
            morphAnimator.SetTrigger("RockToScissors");
            StartCoroutine(CountMorphTime(3));
            if(hasTypeUI) typeHUD.ChangeTypeUI(1);
        } 
        if (currentType == PlayerType.Paper)
        {
            morphAnimator.SetTrigger("PaperToScissors");
            StartCoroutine(CountMorphTime(1));
            if(hasTypeUI) typeHUD.ChangeTypeUI(3);
        }

        yield return new WaitForSeconds(0.15f);

        morphShape.SetActive(false);

        scissorVisual.rotation = Quaternion.Euler(0, 0, 0);
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        currentSkinnedMeshRenderer.enabled = true;
        if(isRespawning){
         currentSkinnedMeshRenderer.enabled = false;
         scissorsCollider.enabled = false;
        }
        else{
            scissorsCollider.enabled = true;
        }
        currentType = PlayerType.Scissors;
        playerScissorsObject.SetActive(true);
        playerRockObject.SetActive(false);
        playerPaperObject.SetActive(false);
        rockCollider.enabled = false;
        paperCollider.enabled = false;
        scissorsCollider.enabled = true;
        StartCoroutine(CountTransformCooldown());
        //Debug.Log("You are now Scissors!");
    }

    void GetALife()
    {
        lives++;
        //Debug.Log("You got an extra life! You have " + lives + " remaining lives!");
    }

    public void DieOrRespawnFunc()
    {
        StartCoroutine(DieOrRespawn());
    }

    public IEnumerator DieOrRespawn()
    {
        if (lives > 0)
        {
            isRespawning = true;
            lives--;
            //Debug.Log("Player lost a life! Remaining lives: " + lives);

            soundSource.Play();
            CinemachineShake.Instance.ShakeCamera(5f, 1f, CinemachineShake.ShakeType.FADING_OUT);

            if (currentType == PlayerType.Rock) Instantiate(rockDeathVFX, transform.position, transform.rotation);
            if (currentType == PlayerType.Paper) Instantiate(paperDeathVFX, transform.position, transform.rotation);
            if (currentType == PlayerType.Scissors) Instantiate(scissorsDeathVFX, transform.position, transform.rotation);

            currentMeshRenderer.enabled = false;
            currentSkinnedMeshRenderer.enabled = false;
            canTakeDamage = false;
            canSwitchType = false;
            paperMeshRenderer.enabled = false;
            rockMeshRenderer.enabled = false;
            scissorsMeshRenderer.enabled = false;
            currentSkinnedMeshRenderer.enabled = false;
            yield return new WaitForSeconds(respawnTime);
            transform.position = respawnPosition;
            if (currentType == PlayerType.Scissors) currentSkinnedMeshRenderer.enabled = true;
            currentMeshRenderer.enabled = true;
            canSwitchType = true;
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            StartCoroutine(DamageFlash(respawnInvulnerabilityTime / 10));
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            yield return new WaitForSeconds(respawnInvulnerabilityTime / 5);
            canSwitchType = true;
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            StartCoroutine(DamageFlash(respawnInvulnerabilityTime / 10));
            yield return new WaitForSeconds(respawnInvulnerabilityTime / 5);
            canSwitchType = true;
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            StartCoroutine(DamageFlash(respawnInvulnerabilityTime / 10));
            yield return new WaitForSeconds(respawnInvulnerabilityTime / 5);
            canSwitchType = true;
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            StartCoroutine(DamageFlash(respawnInvulnerabilityTime / 10));
            yield return new WaitForSeconds(respawnInvulnerabilityTime / 5);
            canSwitchType = true;
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            StartCoroutine(DamageFlash(respawnInvulnerabilityTime / 10));
            yield return new WaitForSeconds(respawnInvulnerabilityTime / 5);
            currentSkinnedMeshRenderer.enabled = true;
            paperMeshRenderer.enabled = true;
            rockMeshRenderer.enabled = true;
            scissorsMeshRenderer.enabled = true;
            canSwitchType = true;
            canTakeDamage = true;
            isRespawning = false;
        }
        else
        {
            Destroy(gameObject, 2);
            DisableVisuals();
            soundSource.Play();
            canTakeDamage = false;
            canSwitchType = false;
            if (currentType == PlayerType.Rock) Instantiate(rockDeathVFX, transform.position, transform.rotation);
            if (currentType == PlayerType.Paper) Instantiate(paperDeathVFX, transform.position, transform.rotation);
            if (currentType == PlayerType.Scissors) Instantiate(scissorsDeathVFX, transform.position, transform.rotation);
            Time.timeScale = 0.25f;
            yield return new WaitForSeconds(0.5f);
            UnityEngine.Cursor.visible = true;
            isAlive = false;
        }
    }

    void DisableVisuals()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    IEnumerator DamageFlash(float duration)
    {
        currentMeshRenderer.material.color = Color.white;
        foreach (var mat in currentSkinnedMeshRenderer.materials)
        {
            mat.SetFloat("_isDamageFlashing", 1);
        }
        yield return new WaitForSeconds(duration);
        foreach (var mat in currentSkinnedMeshRenderer.materials)
        {
            mat.SetFloat("_isDamageFlashing", 0);
        }
        if(currentType == PlayerType.Rock)
        {
            currentMeshRenderer.material.color = rockOriginalColor;
        }
        else if(currentType == PlayerType.Paper)
        {
            currentMeshRenderer.material.color = paperOriginalColor;
        }
    }

    public void IgnoreDamage()
    {
        //Debug.Log("Damage ignored by type!");
        if(currentType == PlayerType.Rock)
        {
            GameObject shieldVfx = Instantiate(rockShieldVFX, transform.position, transform.rotation);
            shieldVfx.transform.parent = this.transform;
        }
        if (currentType == PlayerType.Paper)
        {
            GameObject shieldVfx = Instantiate(paperShieldVFX, transform.position, transform.rotation);
            shieldVfx.transform.parent = this.transform;
        }
        if (currentType == PlayerType.Scissors)
        {
            GameObject shieldVfx = Instantiate(scissorsShieldVFX, transform.position, transform.rotation);
            shieldVfx.transform.parent = this.transform;
        }
    }

    public IEnumerator CountMorphTime(int morphType)
    {
        morphMaterial.SetInt("_switchType", morphType);
        morphRate = 0;
        morphTime = 0;
        isMorphing = true;

        yield return new WaitForSeconds(0.15f);
        isMorphing = false;    
    }

    public IEnumerator CountTransformCooldown()
    {
        canSwitchType = false;
        yield return new WaitForSeconds(typeChangeCooldown);
        if(!isRespawning) canSwitchType = true;
    }

    public enum PlayerType
    {
        Rock,
        Paper,
        Scissors
    }   
}
