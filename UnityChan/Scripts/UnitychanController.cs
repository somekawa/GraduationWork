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
    }

    // rigidbody���g�p����ړ��v�Z�́AFixedUpdate�𗘗p���Ĉ������ł����Ȃ��悤�ɂ���
    void FixedUpdate()
    {
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
            // ���x�x�N�g�����쐬�i3�����p�jY���W��0.0f�ŕK���Œ肷��
            var speed = new Vector3(moveDir_.x, 0.0f, moveDir_.z);
            // ���x�ɐ��K�������x�N�g���ɁA�ړ����x�������đ������
            rigid_.velocity = speed.normalized * SceneMng.charaRunSpeed;

            // ���W�X�V
            // �L�����N�^�[���ړ������鏈��
            rigid_.MovePosition(rigid_.position + rigid_.velocity * Time.deltaTime);
            // �L���������]��
            transform.rotation = Quaternion.LookRotation(moveDir_);
        }

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