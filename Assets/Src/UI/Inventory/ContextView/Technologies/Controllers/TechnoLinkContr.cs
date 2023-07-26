using ReactivePropsNs;
using UnityEngine;
using UnityWeld.Binding;

namespace Uins
{
    [Binding]
    public class TechnoLinkContr : BindingController<TechnoLinkVmodel>
    {
        private const float ImportantItemRadius = 37;
        private const float NotImportantItemRadius = 30;
        private IPositionSetter _positionSetter;
        private RectTransform _rectTransform;


        //=== Props ===========================================================

        [Binding]
        public bool IsAvailable { get; private set; }

        [Binding]
        public float Rotation { get; private set; }

        [Binding]
        public float SizeDeltaX { get; private set; }

        [Binding]
        public float VisualAnchoredPositionX { get; private set; }

        [Binding]
        public float VisualSizeDeltaX { get; private set; }


        //=== Unity ===========================================================

        private void Awake()
        {
            _rectTransform = transform.GetRectTransform();

            Vmodel.Action(D, vm =>
            {
                var from = vm?.MasterTechnologyItem.Technology.Target?.____GetDebugShortName() ?? "none";
                var to = vm?.SlaveTechnologyItem.Technology.Target?.____GetDebugShortName() ?? "none";
                name = $"{from}>>{to}";
            });

            var isAvailableStream = Vmodel.SubStream(D, vm => vm.IsAvailableRp, false);
            Bind(isAvailableStream, () => IsAvailable);

            var positionStream = Vmodel.Func(D, vm => new Vector2(vm?.MasterTechnologyItem.X ?? 0, vm?.MasterTechnologyItem.Y ?? 0));
            var slavePositionStream = Vmodel.Func(D, vm => new Vector2(vm?.SlaveTechnologyItem.X ?? 0, vm?.SlaveTechnologyItem.Y ?? 0));

            var twoPosStream = positionStream.Zip(D, slavePositionStream);

            var deltaXStream = twoPosStream
                .Func(D, (masterPos, slavePos) => (slavePos - masterPos).magnitude * _positionSetter.GridPitch);
            Bind(deltaXStream, () => SizeDeltaX);

            var rotationStream = twoPosStream
                .Func(D, (masterPos, slavePos) =>
                {
                    var rotationAngles = Quaternion.FromToRotation(Vector3.right, (slavePos - masterPos).normalized).eulerAngles;
                    return rotationAngles.y > 0.1 ? rotationAngles.y : rotationAngles.z; //для угла 180 eulerAngles будут не (0,0,180), а (0,180,0)
                });
            Bind(rotationStream, () => Rotation);

            positionStream.Action(D, v2 => _positionSetter?.SetPosition(v2, _rectTransform));

            //Визуальная часть (линия со стрелкой по центру) Stretch-отцентрирована по бокам
            //Отступ визуала слева
            var leftOffsetStream = Vmodel.Func(D,
                vm => (vm?.MasterTechnologyItem.Technology.Target?.IsImportant ?? false) ? ImportantItemRadius : NotImportantItemRadius);

            //Отступ визуала справа
            var rightOffsetStream = Vmodel.Func(D,
                vm => (vm?.SlaveTechnologyItem.Technology.Target?.IsImportant ?? false) ? ImportantItemRadius : NotImportantItemRadius);

            //Из 2 отступов вычисляем X-составляющие AnchoredPosition и SizeDelta для RectTransform визуала
            var visualAnchoredPositionXStream = leftOffsetStream
                .Zip(D, rightOffsetStream)
                .Func(D, (leftOffset, rightOffset) => (leftOffset - rightOffset) / 2);
            Bind(visualAnchoredPositionXStream, () => VisualAnchoredPositionX);

            var visualSizeDeltaXStream = leftOffsetStream
                .Zip(D, rightOffsetStream)
                .Func(D, (leftOffset, rightOffset) => (-leftOffset - rightOffset));
            Bind(visualSizeDeltaXStream, () => VisualSizeDeltaX);
        }


        //=== Public ==========================================================

        public void InitBeforeSetVmodel(IPositionSetter positionSetter)
        {
            if (positionSetter.AssertIfNull(nameof(positionSetter)))
                return;

            _positionSetter = positionSetter;
        }
    }
}