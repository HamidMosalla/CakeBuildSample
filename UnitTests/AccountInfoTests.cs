using System;
using EVisionExercise;
using FakeItEasy;
using FluentAssertions;
using Xunit;

namespace UnitTests
{
    public class AccountInfoTests
    {
        private readonly IAccountService _fakeAccountService;
        private readonly int _accountId;
        private readonly AccountInfo _sut;

        public AccountInfoTests()
        {
            _fakeAccountService = A.Fake<IAccountService>();
            _accountId = 2;
            _sut = new AccountInfo(_accountId, _fakeAccountService);
        }

        [Fact]
        public void RefreshAmount_WhenCalled_SuccessfulyCallsGetAccountAmountMethod()
        {
            _sut.RefreshAmount();

            A.CallTo(() => _fakeAccountService.GetAccountAmount(A<int>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RefeshAmount_WhenCalled_CorrectlySetTheAmountValue()
        {
            const double amount = 4.0;
            A.CallTo(() => _fakeAccountService.GetAccountAmount(A<int>.Ignored)).Returns(amount);

            _sut.RefreshAmount();

            _sut.Amount.Should().Be(amount);
        }

        [Fact]
        public void RefeshAmount_WhenCalled_PassesTheCorrectAccountIdToGetAccountAmountMethod()
        {
            _sut.RefreshAmount();

            A.CallTo(() => _fakeAccountService.GetAccountAmount(A<int>.That.Matches(a => a == _accountId)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
