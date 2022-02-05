using UnityEngine;

// �L�����̊�{�ړ��ƕ����]���݂̂�Script
// ���퓬�pScript�͕�

public class UnitychanController : MonoBehaviour
{
    private Rigidbody rigid_;      // Rigidbody�R���|�[�l���g
    private Animator animator_;    // Animator �R���|�[�l���g

    // �A�j���[�V����
    private readonly int runParamHash_ = Animator.StringToHash("isRun");
    private readonly int attackParamHash_ = Animator.StringToHash("isAttack");

    // ������Ԃ��m�F�������L�[���܂Ƃ߂�����
    private KeyCode[] keyArray_ = new KeyCode[4] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };

    private Vector3 moveDir_ = Vector3.zero;

    private Ray ray; // ��΂����C
    private float distance = 0.5f; // ���C���΂�����
    private RaycastHit hit; // ���C�������ɓ����������̏��
    private bool isGroundFlg_;

    private GameObject lodingCanvasBackImage_;  // ���[�h��ʂ̔w�i���擾���Ă���

    void Start()
    {
        rigid_ = GetComponent<Rigidbody>();
        animator_= GetComponent<Animator>();
    }

    void Update()
    {
        // �T�����ȊO�̓A�j���[�V�������ς��Ȃ��悤�ɂ���
        if (FieldMng.nowMode != FieldMng.MODE.SEARCH)
        {
            return;
        }

         bool tmpFlg = false;       // ���W�ړ��̃{�^����������true�ɂȂ�
        foreach (KeyCode i in keyArray_)
        {
            // keyArray_�ɐݒ肵��KeyCode�̒��ŁA��������Ă���{�^�������邩�𒲂ׂ�
            if (Input.GetKey(i))
            {
                // Wait����Run�ɑJ�ڂ���
                this.animator_.SetBool(runParamHash_, true);
                tmpFlg = true;
                break;  // ����ȏ�񂷕K�v���Ȃ��̂ŁAbreak�Ŕ�����
            }
        }

        if (!tmpFlg) // ���foreach��ʂ��Ă�false�̂܂܂������ꍇ�́A����A�j���[�V������false�ɂ���(=�ҋ@)
        {
            // Run����Wait�ɑJ�ڂ���
            this.animator_.SetBool(runParamHash_, false);
            return; // �ҋ@�A�j���[�V�����Ƃ������Ƃ͉��̍��W�ړ��������s���K�v���Ȃ����߁Areturn����
        }

        // ��󉺃{�^�����������Ă���
        if (Input.GetKey(keyArray_[0]) || Input.GetKey(keyArray_[1]))
        {
            // ��L�[ or ���L�[
            moveDir_.z = Input.GetAxis("Vertical");
        }

        if (Input.GetKey(keyArray_[2]) || Input.GetKey(keyArray_[3]))
        {
            // ���L�[ or �E�L�[
            moveDir_.x = Input.GetAxis("Horizontal");
        }

        ray = new Ray(transform.position, transform.up * -1); // ���C�����ɔ�΂�
        //Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 1.0f); // ���C��ԐF�ŕ\��������

        if (Physics.Raycast(ray, out hit, distance)) // ���C�������������̏���
        {
            isGroundFlg_ = true;
            //Debug.Log("�n�ʂɐG�ꂽ");
        }
        else
        {
            isGroundFlg_ = false;
            rigid_.velocity += Vector3.down * 2.0f;
            rigid_.MovePosition(rigid_.position + rigid_.velocity * Time.deltaTime);
            //Debug.Log("�n�ʂɐG��ĂȂ�");
            //Debug.Log(rigid_.position + rigid_.velocity * Time.deltaTime);
        }
    }

    // rigidbody���g�p����ړ��v�Z�́AFixedUpdate�𗘗p���Ĉ������ł����Ȃ��悤�ɂ���
    void FixedUpdate()
    {
        // ���[�h��ʒ��ɃL�������ړ�����ƁA�����V�[���̓ǂݍ��݃o�O����������ׁA�~�߂���悤�ɂ���
        if(lodingCanvasBackImage_ == null)
        {
            lodingCanvasBackImage_ = GameObject.Find("LoadingCanvas/BackImage").gameObject;
        }

        if(lodingCanvasBackImage_.activeSelf)
        {
            Debug.Log("���[�h���̂��߁A�ړ��s��");
            return;
        }

        // �T�����[�h�ȊO�Ŏ��R�ɓ����ꂽ�炢���Ȃ��̂ŁAreturn������������B
        if (FieldMng.nowMode != FieldMng.MODE.SEARCH)
        {
            // �L�����ɂ������Ă��銵�����ꎞ�I�Ɏ~�߂�
            rigid_.velocity = Vector3.zero;
            rigid_.angularVelocity = Vector3.zero;

            // ������Run�̃A�j���[�V������ύX���Ă����Ȃ��ƁA���[�h���؂�ւ��u�Ԃ܂ő����Ă�����
            // ���胂�[�V�������퓬���Ɍp�����Ă��܂��B
            this.animator_.SetBool(runParamHash_, false);
            return;
        }
        else
        {
            // �퓬����T���ɖ߂��Ă����Ƃ��ɁA�U�����[�V�����̓r���Ȃ�؂�グ��悤�ɂ���
            this.animator_.SetBool(attackParamHash_, false);
        }

        // �O���[�o�����W�ɕϊ�����ƁA�L�����̕����]�����+-���o�O���N����
        //Vector3 globaldir = transform.TransformDirection(movedir);
        //controller_.Move(globaldir * Time.deltaTime);

        if (moveDir_ != Vector3.zero)
        {
            var speed = new Vector3(moveDir_.x, 0.0f, moveDir_.z);
            // ���x�ɐ��K�������x�N�g���ɁA�ړ����x�������đ������
            rigid_.velocity = speed.normalized * SceneMng.charaRunSpeed;

            // ���W�X�V
            // �L�����N�^�[���ړ������鏈��
            rigid_.MovePosition(rigid_.position + rigid_.velocity * Time.deltaTime);
            // �L���������]��
            transform.rotation = Quaternion.LookRotation(moveDir_);
        }
        else
        {
            if(!isGroundFlg_)
            {
                rigid_.velocity += Vector3.down * 2.0f;
                rigid_.MovePosition(rigid_.position + rigid_.velocity * Time.deltaTime);
            }
            else
            {
                // �L�����ɂ������Ă��銵�����~�߂�
                rigid_.velocity = Vector3.zero;
                rigid_.angularVelocity = Vector3.zero;
            }
        }

        isGroundFlg_ = true;
        moveDir_ = Vector3.zero;
        rigid_.velocity = Vector3.zero;
    }

    // �L�������ړ�����
    public bool GetMoveFlag()
    {
        return this.animator_.GetBool(runParamHash_);
    }

    // �L�����̑���A�j���[�V�������~�߂�
    public void StopUniRunAnim()
    {
        // �A�j���[�V���������邩null�`�F�b�N���s��
        if(this.animator_ != null)
        {
            this.animator_.SetBool(runParamHash_, false);
        }
    }
}