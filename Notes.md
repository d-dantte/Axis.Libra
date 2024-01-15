
2021/Dec/17 ===============================================================

1. Since commands are by design, asynchronious - i.e, each command execution only returns the Command Signature which is used to query for the result,
   Every command must come with a corresponding/counterpart Query which will be used to retrieve the result of the command.


2022/Sept/06 ==============================================================

1. Disallow multiple registrations for a specific Command/Query/Request handler.
	1. Registrar methods will be changed from `AddxxxHandlerRegistration` to `RegisterxxxHandler`.
	2. Duplicate registrations will throw exceptions.
2. Consider using the command/query/request manifest as the input into the dispatchers, and subsequently,
   the means to resolve registrations. This means the manifests will in turn hold a reference to the ServiceResolver,
   and each resolution call will in turn be passed unto the internal resolver.


2022/Sept/19 ===========================================================

1. Make sure that all instruction namespaces within a Query/Request/Command Registrar are unique.

2024/Jan/15 ============================================================

1.  Make Command/Query/Request Dispatchers interfaces, and then implement the interfaces.
2.  Kill the ICommand/IQuery/IRequest interfaces. The goal here is to keep the coupling users have with Axis.Libra to
    only the dependency on the dispatchers.
3.  As a consequence of #2, while doing the mandatory Command/Query/Request registrations with the respective manifests,
    registration will require 2 new arguments:
    a. Command/Query/Request namespace, in the form of a string - converted to the appropriate type internally.
    b. A function that accepts an instance of the Command/Query/Request, and returns a ulong hash of it`s relevant state.
    
    Together, the above is used to generate the `InstructionUri`, a unique identifier for the interaction. This identifier
    can be cached, along with the result, so subsequent calls need not go through to the Command/Query/Request handler.
4.  Implement support for `Instruction caching` - described in #3 above, this is a situation where a dispatcher receives
    an instruction, extracts the `InstructionUri`, and checks a cache if that uri has a response. If it does, it returns
    the response.
