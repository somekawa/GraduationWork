using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneMng : MonoBehaviour
{
    //�v���C���[��ϐ��Ɋi�[
    public GameObject Player;

    //��]������X�s�[�h
    public float rotateSpeed = 0.5f;

    public Camera camera;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("�J�����𓮂����܂�");
       //// StartCoroutine(MoveCamera());
    }

    void Update()
    {
        camera.transform.RotateAround(Player.transform.position, new Vector3(0, 1, 0), 0.5f);
    }

    private IEnumerator MoveCamera()
    {
        while (true)
        {
            yield return null;
            //��]������p�x
            float angle = Input.GetAxis("Horizontal") * rotateSpeed;

            //�v���C���[�ʒu���
            Vector3 playerPos = Player.transform.position;

            //�J��������]������
            transform.RotateAround(playerPos, Vector3.up, angle);

            // ���{�^���������X�y�[�X�L�[�����Ń��x���A�b�v�p�̉摜��\��
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("�R���[�`�����~�߂܂���");
                yield break;
            }

        }
    }

    public void OnClickNewGame()
    {
        SceneManager.LoadScene("conversationdata");
    }

    public void OnClickLoadGame()
    {
        SceneManager.LoadScene("Town");
    }
}
