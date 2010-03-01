namespace FluentValidation.Syntax {
	using Internal;

	public class CascadeStep<T,TProperty> {
		readonly IRuleBuilderInitial<T, TProperty> parent;

		public CascadeStep(IRuleBuilderInitial<T, TProperty> parent) {
			this.parent = parent;
		}

		public IRuleBuilderInitial<T,TProperty> Continue() {
			return parent.Configure(x => x.CascadeMode = CascadeMode.Continue);
		}

		public IRuleBuilderInitial<T,TProperty> StopOnFirstFailure() {
			return parent.Configure(x => x.CascadeMode = CascadeMode.StopOnFirstFailure);
		}
	}
}