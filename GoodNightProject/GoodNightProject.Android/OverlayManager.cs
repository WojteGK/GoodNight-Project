using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GoodNightProject.Droid
{
    public class OverlayManager
    {
        private Context _context;
        private WindowManagerLayoutParams _layoutParams;
        private IWindowManager _androidWindowManager;
        private View _overlayView;

        public void Initialize(Context context)
        {
            _context = context;
            InitializeOverlay();
        }

        private void InitializeOverlay()
        {
            _androidWindowManager = _context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            _layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent,
                WindowManagerTypes.ApplicationOverlay,
                WindowManagerFlags.NotFocusable,
                Format.Translucent);

            _layoutParams.Flags = WindowManagerFlags.LayoutNoLimits;

            _overlayView = LayoutInflater.From(_context).Inflate(Resource.Layout.notification_layout, null);
        }

        public void ShowOverlay()
        {
            _androidWindowManager.AddView(_overlayView, _layoutParams);
        }

        public void HideOverlay()
        {
            if (_overlayView != null)
            {
                _androidWindowManager.RemoveView(_overlayView);
            }
        }
    }
}
