using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public old_Unit shooter;
    private Rigidbody rb;
    public float velocity = 10;
    public Weapon weapon = null;
    public Vector3 path_direction;
    public float fire_delay = 0f;
    public float windup_delay = 0f;
    public UISoundManager.SoundEffectSet shotEffectType = UISoundManager.SoundEffectSet.ArrowFire;
    public UISoundManager.SoundEffectSet windupEffectType = UISoundManager.SoundEffectSet.BowDraw;
    public UISoundManager.SoundEffectSet impactEffectType = UISoundManager.SoundEffectSet.Stab;
    public bool has_fired = false;
    public float linger_time = 0;
    public float rotation_rate = 0;
    public Vector3 offset_transform = Vector3.zero;
    public old_Unit target;
    public UISoundManager soundManager;
    public Vector3 offset = Vector3.zero;

    public int cohesionDamage = 0;
    public int killDamage = 0;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Renderer>().enabled = false;
    }

    public void SetupSound(UISoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Time.time < fire_delay)
        {
            GetComponent<Renderer>().enabled = false;

        }
        else if(Time.time < fire_delay + windup_delay)
        {
            AudioClip windupClip = soundManager.GetClipFromSet(windupEffectType);
            transform.position = shooter.transform.position + offset;
            if (windupClip != null)
            {
                GetComponent<Renderer>().enabled = true;
                AudioSource.PlayClipAtPoint(windupClip, transform.position, 0.16f);
            }
        }
        else
        {
            if (!has_fired)
            {
                AudioClip shotClip = soundManager.GetClipFromSet(shotEffectType);
                if (shotClip != null)
                {
                    AudioSource.PlayClipAtPoint(shotClip, transform.position, 1.3f);
                }
                GetComponent<Renderer>().enabled = true;
                has_fired = true;

                float done_time = (weapon.range * old_BattleEngine.meters_to_game_units) / (velocity) + linger_time;
                
                GameObject.Destroy(gameObject, done_time);
            }
            transform.position = transform.position + path_direction * velocity * Time.deltaTime;

            // If its a slashing weapon, rotate as a slash
            if (rotation_rate > 0)
            {
                transform.Rotate(Vector3.forward, rotation_rate * Time.deltaTime);
            }
        }
    }

    // function to launch the bullet
    public void Shoot()
    {
        Vector3 Direction = target.transform.position - transform.position;
        float minR = -weapon.recoil;
        float maxR = weapon.recoil;

        float recoil = Random.Range(minR, maxR);
        path_direction = (Quaternion.AngleAxis(recoil, Vector3.forward) * Direction).normalized;

        // random velocity
        velocity = Random.Range(0.9f, 1.15f) * velocity;

        // handles when trying to look at nothing, vector length 0
        if (target.transform.position != transform.position)
        {
            Quaternion rotation = Quaternion.LookRotation(path_direction, transform.TransformDirection(-Vector3.forward));
            rotation = new Quaternion(0, 0, rotation.z, rotation.w);
            transform.rotation = rotation;
        }
    }

    // bullet hits something
    void OnTriggerEnter(Collider collision)
    {
        GameObject t = collision.gameObject;
        old_Battalion hitUnit = t.GetComponent<old_Battalion>();
        if (hitUnit != null)
        {
            if (hitUnit == target)
            {

                GameObject.Destroy(gameObject, 0.05f+ linger_time);
                AudioClip impactClip = soundManager.GetClipFromSet(impactEffectType);
                if (impactClip != null)
                {
                    AudioSource.PlayClipAtPoint(impactClip, transform.position, 0.5f);
                }
                target.battalion.men -= killDamage;
                shooter.battalion.kills += killDamage;
                target.battalion.cohesion -= cohesionDamage;
            }
            else if(hitUnit != shooter && hitUnit.battlegroup.team == shooter.battalion.battlegroup.team)
            {
                killDamage = old_BattleEngine.RandomRound(killDamage * old_BattleEngine.impededAttackPenalty);
                cohesionDamage = old_BattleEngine.RandomRound(cohesionDamage * old_BattleEngine.impededAttackPenalty);
                
            }
        }
    }
    
    
}
