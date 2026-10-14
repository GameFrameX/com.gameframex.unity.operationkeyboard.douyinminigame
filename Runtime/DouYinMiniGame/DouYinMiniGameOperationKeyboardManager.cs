#if UNITY_WEBGL && ENABLE_DOUYIN_MINI_GAME && ENABLE_DOUYIN_MINI_GAME_OPERATIONKEYBOARD

// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相关法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root of the source tree for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any disputes and liabilities arising from secondary development based on this project
//   本项目组织及贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:    https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using GameFrameX.OperationKeyboard.Runtime;

namespace GameFrameX.OperationKeyboard.DouYinMiniGame.Runtime
{
    /// <summary>
    /// 抖音小游戏键盘管理器。
    /// </summary>
    /// <remarks>
    /// 直通 StarkSDK 的 TT.ShowKeyboard / TT.HideKeyboard / TT.UpdateKeyboard API；Hide 时注销全部平台事件；
    /// 事件单槽回调由 OperationKeyboard 门面在字段会话内统一登记。
    /// Pass-through adapter over StarkSDK keyboard APIs; all platform events are unsubscribed on Hide;
    /// single-slot callbacks are registered by the OperationKeyboard facade per field session.
    /// </remarks>
    [UnityEngine.Scripting.Preserve]
    public sealed class DouYinMiniGameOperationKeyboardManager : IOperationKeyboardManager
    {
        /// <summary>
        /// 渠道唯一标识
        /// </summary>
        public const string ChannelId = "douyinminigame";

        private Action<string> _onInput;
        private Action<string> _onConfirm;
        private Action<string> _onComplete;
        private Action<float> _onHeightChanged;

        private bool _listening;
        private TTSDK.TTKeyboard.OnKeyboardInputEvent _onKeyboardInput;
        private TTSDK.TTKeyboard.OnKeyboardInputEvent _onKeyboardConfirm;
        private TTSDK.TTKeyboard.OnKeyboardInputEvent _onKeyboardComplete;

        /// <summary>
        /// 显示键盘。
        /// </summary>
        /// <param name="option">显示选项。</param>
        /// <remarks>
        /// 仅设置官方示例中出现的 defaultValue / maxLength / multiple / confirmType 四个字段；
        /// maxLength 契约恒为 -1（不限），与微信同构直传，真机回归项。
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void Show(OperationKeyboardShowOption option)
        {
            if (option == null)
            {
                return;
            }

            TTSDK.TT.ShowKeyboard(new TTSDK.TTKeyboard.ShowKeyboardOptions
            {
                defaultValue = option.DefaultValue,
                maxLength = option.MaxLength,
                multiple = option.Multiple,
                confirmType = option.ConfirmType,
            }, null, null);

            EnsureListeners();
        }

        /// <summary>
        /// 隐藏键盘并注销全部平台事件。
        /// </summary>
        [UnityEngine.Scripting.Preserve]
        public void Hide()
        {
            if (!_listening)
            {
                return;
            }

            TTSDK.TT.HideKeyboard(null, null);

            TTSDK.TT.OnKeyboardInput -= _onKeyboardInput;
            TTSDK.TT.OnKeyboardConfirm -= _onKeyboardConfirm;
            TTSDK.TT.OnKeyboardComplete -= _onKeyboardComplete;

            _onInput = null;
            _onConfirm = null;
            _onComplete = null;
            _onHeightChanged = null;
            _listening = false;
        }

        /// <summary>
        /// 更新键盘输入框内容。
        /// </summary>
        /// <param name="value">新的文本内容。</param>
        [UnityEngine.Scripting.Preserve]
        public void UpdateValue(string value)
        {
            TTSDK.TT.UpdateKeyboard(value, null, null);
        }

        /// <summary>
        /// 登记输入事件回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnInput(Action<string> handler)
        {
            _onInput = handler;
        }

        /// <summary>
        /// 登记确认事件回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnConfirm(Action<string> handler)
        {
            _onConfirm = handler;
        }

        /// <summary>
        /// 登记编辑完成事件回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调。</param>
        [UnityEngine.Scripting.Preserve]
        public void OnComplete(Action<string> handler)
        {
            _onComplete = handler;
        }

        /// <summary>
        /// 登记键盘高度变化回调（单槽，后设覆盖先设）。
        /// </summary>
        /// <param name="handler">回调。</param>
        /// <remarks>
        /// 抖音无键盘高度 API，按契约空实现。StarkSDK exposes no keyboard-height API, so this is a no-op per contract.
        /// </remarks>
        [UnityEngine.Scripting.Preserve]
        public void OnHeightChanged(Action<float> handler)
        {
            _onHeightChanged = handler;
        }

        private void EnsureListeners()
        {
            if (_listening)
            {
                return;
            }

            _onKeyboardInput = OnKeyboardInput;
            _onKeyboardConfirm = OnKeyboardConfirm;
            _onKeyboardComplete = OnKeyboardComplete;

            TTSDK.TT.OnKeyboardInput += _onKeyboardInput;
            TTSDK.TT.OnKeyboardConfirm += _onKeyboardConfirm;
            TTSDK.TT.OnKeyboardComplete += _onKeyboardComplete;
            _listening = true;
        }

        private void OnKeyboardInput(string value)
        {
            _onInput?.Invoke(value);
        }

        private void OnKeyboardConfirm(string value)
        {
            _onConfirm?.Invoke(value);
        }

        private void OnKeyboardComplete(string value)
        {
            _onComplete?.Invoke(value);
        }
    }
}

#endif
