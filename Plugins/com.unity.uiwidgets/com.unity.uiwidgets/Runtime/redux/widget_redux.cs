using System;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Unity.UIWidgets.Redux {
    public class StoreProvider<State> : InheritedWidget {
        readonly Store<State> _store;

        public StoreProvider(
            Store<State> store = null,
            Widget child = null,
            Key key = null) : base(key: key, child: child) {
            D.assert(store != null);
            D.assert(child != null);
            _store = store;
        }

        public static Store<State> of(BuildContext context) {
            var type = _typeOf<StoreProvider<State>>();
            StoreProvider<State> provider = context.inheritFromWidgetOfExactType(type) as StoreProvider<State>;
            if (provider == null) {
                throw new UIWidgetsError("StoreProvider is missing");
            }

            return provider._store;
        }

        static Type _typeOf<T>() {
            return typeof(T);
        }

        public override bool updateShouldNotify(InheritedWidget old) {
            return !Equals(objA: _store, objB: ((StoreProvider<State>) old)._store);
        }
    }

    public delegate Widget ViewModelBuilder<in ViewModel>(BuildContext context, ViewModel viewModel, Dispatcher dispatcher);

    public delegate ViewModel StoreConverter<in State, out ViewModel>(State state);

    public delegate bool ShouldRebuildCallback<in ViewModel>(ViewModel previous, ViewModel current);

    public class StoreConnector<State, ViewModel> : StatelessWidget {
        public readonly ViewModelBuilder<ViewModel> builder;

        public readonly StoreConverter<State, ViewModel> converter;

        public readonly ShouldRebuildCallback<ViewModel> shouldRebuild;

        public readonly bool pure;

        public StoreConnector(
            ViewModelBuilder<ViewModel> builder = null,
            StoreConverter<State, ViewModel> converter = null,
            bool pure = false, 
            ShouldRebuildCallback<ViewModel> shouldRebuild = null,
            Key key = null) : base(key) {
            D.assert(builder != null);
            D.assert(converter != null);
            this.pure = pure;
            this.builder = builder;
            this.converter = converter;
            this.shouldRebuild = shouldRebuild;
        }

        public override Widget build(BuildContext context) {
            return new _StoreListener<State, ViewModel>(
                store: StoreProvider<State>.of(context),
                builder: builder,
                converter: converter,
                pure: pure,
                shouldRebuild: shouldRebuild
            );
        }
    }

    public class _StoreListener<State, ViewModel> : StatefulWidget {
        public readonly ViewModelBuilder<ViewModel> builder;

        public readonly StoreConverter<State, ViewModel> converter;

        public readonly Store<State> store;

        public readonly ShouldRebuildCallback<ViewModel> shouldRebuild;

        public readonly bool pure;

        public _StoreListener(
            ViewModelBuilder<ViewModel> builder = null,
            StoreConverter<State, ViewModel> converter = null,
            Store<State> store = null,
            bool pure = false,
            ShouldRebuildCallback<ViewModel> shouldRebuild = null,
            Key key = null) : base(key) {
            D.assert(builder != null);
            D.assert(converter != null);
            D.assert(store != null);
            this.store = store;
            this.builder = builder;
            this.converter = converter;
            this.pure = pure;
            this.shouldRebuild = shouldRebuild;
        }

        public override widgets.State createState() {
            return new _StoreListenerState<State, ViewModel>();
        }
    }

    class _StoreListenerState<State, ViewModel> : State<_StoreListener<State, ViewModel>> {
        ViewModel latestValue;

        public override void initState() {
            base.initState();
            _init();
        }

        public override void dispose() {
            widget.store.stateChanged -= _handleStateChanged;
            base.dispose();
        }

        public override void didUpdateWidget(StatefulWidget oldWidget) {
            var oldStore = ((_StoreListener<State, ViewModel>) oldWidget).store;
            if (widget.store != oldStore) {
                oldStore.stateChanged -= _handleStateChanged;
                _init();
            }

            base.didUpdateWidget(oldWidget);
        }

        void _init() {
            widget.store.stateChanged += _handleStateChanged;
            latestValue = widget.converter(widget.store.getState());
        }

        void _handleStateChanged(State state) {
            if (Window.instance._panel != null) {
                _innerStateChanged(state: state);
            }
            else {
                var isolate = Isolate.current;
                using (Isolate.getScope(isolate: isolate)) {
                    _innerStateChanged(state: state);
                }
            }
        }

        void _innerStateChanged(State state) {
            var preValue = latestValue;
            latestValue = widget.converter(widget.store.getState());
            if (widget.shouldRebuild != null) {
                if (!widget.shouldRebuild(preValue, latestValue)) {
                    return;
                }
            }
            else if (widget.pure) {
                if (Equals(preValue, latestValue)) {
                    return;
                }
            }

            setState();
        }

        public override Widget build(BuildContext context) {
            return widget.builder(context, latestValue, widget.store.dispatcher);
        }
    }
}