using UnityEngine;

public class Free2MoveCamera : MonoBehaviour
{
    [SerializeField]private float moveSpeed = 10f;
    [SerializeField] private GameObject lookAt;
    
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var direction = new Vector3(horizontal, 0, vertical);
        transform.Translate(direction * (moveSpeed * Time.deltaTime));
        transform.LookAt(lookAt.transform);

    }

}
