using Gameplay.Rewards;
using TMPro;
using System.Collections;
using UnityEngine;

namespace Gameplay.UI
{
    public class UIMoneyRewardBreakdown : MonoBehaviour
    {
        private const string LevelRewardLabel = "Level reward";
        private const string BaseHealthBonusLabel = "Base HP bonus";
        private const string TimeBonusLabel = "Time bonus";
        private const string TotalLabel = "Total";

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private UIMoneyRewardRow _levelRewardRow;
        [SerializeField] private UIMoneyRewardRow _baseHealthBonusRow;
        [SerializeField] private UIMoneyRewardRow _timeBonusRow;
        [SerializeField] private UIMoneyRewardRow _totalRow;

        [SerializeField] private float _rowAppearDuration = 0.16f;
        [SerializeField] private float _rowDelay = 0.12f;
        [SerializeField] private float _totalCountDuration = 0.45f;

        private VictoryRewardBreakdown _rewards;

        public void Initialize(VictoryRewardBreakdown rewards)
        {
            _rewards = rewards;
            gameObject.SetActive(true);

            if (_titleText != null)
                _titleText.text = "Coins earned";

            if (_levelRewardRow != null)
                _levelRewardRow.Initialize(LevelRewardLabel, rewards.LevelReward);

            if (_baseHealthBonusRow != null)
                _baseHealthBonusRow.Initialize(BaseHealthBonusLabel, rewards.BaseHealthBonus);

            if (_timeBonusRow != null)
                _timeBonusRow.Initialize(TimeBonusLabel, rewards.TimeBonus);

            if (_totalRow != null)
                _totalRow.Initialize(TotalLabel, rewards.TotalMoney);
        }

        public IEnumerator PlaySequence()
        {
            PrepareRows();

            yield return PlayRow(_levelRewardRow);
            yield return PlayRow(_baseHealthBonusRow);
            yield return PlayRow(_timeBonusRow);

            if (_totalRow != null)
                yield return _totalRow.PlayTotalCount(_rewards.TotalMoney, _rowAppearDuration, _totalCountDuration);
        }

        private void PrepareRows()
        {
            _levelRewardRow?.PrepareHidden();
            _baseHealthBonusRow?.PrepareHidden();
            _timeBonusRow?.PrepareHidden();
            _totalRow?.PrepareHidden();
        }

        private IEnumerator PlayRow(UIMoneyRewardRow row)
        {
            if (row == null)
                yield break;

            yield return row.PlayAppear(_rowAppearDuration);
            yield return new WaitForSecondsRealtime(_rowDelay);
        }
    }
}
