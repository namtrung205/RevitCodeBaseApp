using System ;
using System.Linq ;
using System.Linq.Expressions ;

namespace Helpers.Utils ;

public static class UtAttribute
{
  public static object? GetValueAttribute<TAttribute>( this object value )
    where TAttribute : Attribute
  {
    var fi = value.GetType().GetField( value.ToString() ) ;
    var attributes = fi.GetCustomAttributes( typeof( TAttribute ), false ) as TAttribute[] ;
    if ( attributes == null ) return null ;
    if ( attributes.Length > 0 ) {
      var fullName = typeof( TAttribute ).FullName ;
      if ( fullName == null ) return null ;
      var listSplitName = fullName.Split( '.' ) ;
      string? nameProperty ;
      if ( listSplitName.Length == 1 )
        nameProperty = fullName ;
      else
        nameProperty = listSplitName.FirstOrDefault( x => x.Contains( "Attribute" ) ) ?? listSplitName.FirstOrDefault() ;
      if ( nameProperty == null ) return null ;
      var propertyName = nameProperty.Replace( "Attribute", string.Empty ) ;
      var result = attributes[ 0 ].GetType().GetProperty( propertyName )?.GetValue( attributes[ 0 ] ) ;
      return result ;
    }

    return null ;
  }

  public static object? GetValueAttribute<TAttribute>( Expression<Func<object>> propertyExpression )
    where TAttribute : Attribute
  {
    try {
      var type = ( propertyExpression.Body as MemberExpression )?.Expression.Type ??
                 ( ( propertyExpression.Body as UnaryExpression )?.Operand as MemberExpression )?.Expression.Type ;
      var name = ( propertyExpression.Body as MemberExpression )?.Member.Name ??
                 ( ( propertyExpression.Body as UnaryExpression )?.Operand as MemberExpression )?.Member.Name ;
      if ( type == null || name == null ) return null ;
      if ( type.GetProperty( name )?.GetCustomAttributes( typeof( TAttribute ), false ).FirstOrDefault() is not TAttribute attribute ) return null ;
      var fullName = typeof( TAttribute ).Name ;
      var listSplitName = fullName.Split( '.' ) ;
      string? nameProperty ;
      if ( listSplitName.Length == 1 )
        nameProperty = fullName ;
      else
        nameProperty = listSplitName.FirstOrDefault( x => x.Contains( "Attribute" ) ) ?? listSplitName.FirstOrDefault() ;
      if ( nameProperty == null ) return null ;
      var propertyName = nameProperty.Replace( "Attribute", string.Empty ) ;
      var result = attribute.GetType().GetProperty( propertyName )?.GetValue( attribute ) ;
      return result ;
    }
    catch ( System.Exception e ) {
     LoggerCit.Instance.LogError (e) ;
      return null ;
    }
  }

  // public static TAttributeCustom GetAttributeCustom<TParent, TAttributeCustom>( Expression<Func<TParent>> propertyExpression ) where TParent : class
  // {
  //   var name = (propertyExpression.Body as MemberExpression)?.Member.Name! ;
  //   return (TAttributeCustom) typeof( TParent ).GetProperty( name )!.GetCustomAttributes( typeof( TAttributeCustom ), false ).FirstOrDefault()! ;
  // }
}